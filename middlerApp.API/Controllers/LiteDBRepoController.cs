using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using middler.Action.Scripting;
using middler.Common.Actions.UrlRedirect;
using middler.Common.Actions.UrlRewrite;
using middler.Common.Storage;
using middler.Core;
using middler.Core.ExtensionMethods;
using middler.Hosting.Models;
using middlerApp.API.Attributes;
using NamedServices.Microsoft.Extensions.DependencyInjection;
using Reflectensions.ExtensionMethods;
using Converter = middlerApp.API.Helper.Converter;

namespace middlerApp.API.Controllers
{
    [ApiController]
    [Route("api/repo/litedb")]
    [AdminController]
    public class LiteDBRepoController: Controller
    {
        private readonly IMapper _mapper;
        private readonly InternalHelper _internalHelper;
        private IMiddlerStorage Repo { get; }

        public LiteDBRepoController(IServiceProvider serviceProvider, IMapper mapper, InternalHelper internalHelper)
        {
            _mapper = mapper;
            _internalHelper = internalHelper;
            Repo = serviceProvider.GetNamedService<IMiddlerStorage>("litedb");
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MiddlerRuleDto>>> GetAll()
        {
            var rules = await Repo.GetAllAsync();

            return Ok(rules.Select(ToDto));
        }


        [HttpPost]
        public async Task<ActionResult> Add([FromBody]CreateMiddlerRuleDto rule) {

            var dbModel = _mapper.Map<MiddlerRuleDbModel>(rule);
            UpdateActions(dbModel);
            await Repo.AddAsync(dbModel);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await Repo.RemoveAsync(id);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MiddlerRuleDto>> Update(Guid id, [FromBody]UpdateMiddlerRuleDto rule)
        {
            var dbModel = _mapper.Map<MiddlerRuleDbModel>(rule);
            dbModel.Id = id;
            UpdateActions(dbModel);
            await Repo.UpdateAsync(dbModel);
            var updated = await Repo.GetByIdAsync(id);
            return Ok(ToDto(updated));
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<MiddlerRuleDto>> PartialUpdate(Guid id, [FromBody]JsonPatchDocument<UpdateMiddlerRuleDto> patchDocument) {

            var ruleInDb = await Repo.GetByIdAsync(id);

            var updDto = _mapper.Map<UpdateMiddlerRuleDto>(ruleInDb);
            patchDocument.ApplyTo(updDto, ModelState);

            _mapper.Map(updDto, ruleInDb);
            UpdateActions(ruleInDb);
            await Repo.UpdateAsync(ruleInDb);
            var updated = await Repo.GetByIdAsync(id);
            return Ok(ToDto(updated));
        }

        [HttpPatch("order")]
        public async Task<ActionResult<MiddlerRuleDto>> OrderRules([FromBody]JsonPatchDocument<UpdateMiddlerRuleDto> patchDocument) {


            foreach (var patchDocumentOperation in patchDocument.Operations)
            {

                var id = patchDocumentOperation.path.TrimStart('/');
                switch (patchDocumentOperation.OperationType)
                {
                    case OperationType.Replace:
                    {
                        var rule = await Repo.GetByIdAsync(id.ToGuid());
                        rule.Order = patchDocumentOperation.value.To<decimal>();
                        await Repo.UpdateAsync(rule);
                        break;
                    }
                }
                
            }

            return Ok();
        }


        private void UpdateActions(MiddlerRuleDbModel ruleDbModel)
        {
            foreach (var middlerAction in ruleDbModel.Actions)
            {
                if (middlerAction.ActionType == "Script")
                {
                    var scriptAction = _internalHelper.BuildConcreteActionInstance(middlerAction) as ScriptingAction;
                    middlerAction.Parameters["CompiledCode"] = scriptAction?.CompileScriptIfNeeded();
                }
            }
        }

        private MiddlerRuleDto ToDto(MiddlerRuleDbModel dbModel)
        {
            var dto = Converter.CopyTo<MiddlerRuleDto>(dbModel);
            dto.Actions = dto.Actions.Select(action =>
            {
                action.Parameters.Remove("CompiledCode");
                return action;
            }).ToList();
            return dto;
        }
    }
}
