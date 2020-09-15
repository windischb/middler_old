using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using middler.Action.Scripting;
using middler.Common.SharedModels.Models;
using middler.Core;
using middlerApp.API.Attributes;
using middlerApp.API.DataAccess;
using middlerApp.SharedModels;

namespace middlerApp.API.Controllers.Admin
{
    [ApiController]
    [Route("api/endpoint-rules/{ruleId}/actions")]
    [AdminController]
    public class RulesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly InternalHelper _internalHelper;
        private readonly EndpointRuleRepository _endpointRuleRepository;

        public RulesController(IMapper mapper, InternalHelper internalHelper, EndpointRuleRepository endpointRuleRepository)
        {
            _mapper = mapper;
            _internalHelper = internalHelper;
            _endpointRuleRepository = endpointRuleRepository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<EndpointActionDto>>> GetActionsForRule(Guid ruleId)
        {
            var rules = await _endpointRuleRepository.GetActionsForRuleAsync(ruleId);
            return Ok(_mapper.Map<List<EndpointActionDto>>(rules));
        }

        [HttpPost]
        public async Task<ActionResult> AddActionToRule(Guid ruleId, EndpointActionDto actionDto)
        {
            actionDto.EndpointRuleEntityId = ruleId;
            var entity = _mapper.Map<EndpointActionEntity>(actionDto);
            UpdateAction(entity);
            await _endpointRuleRepository.AddActionToRule(entity);

            return Ok();
        }


        [HttpPatch("{id}")]
        public async Task<ActionResult<EndpointRuleEntity>> PartialUpdate(Guid id, [FromBody] JsonPatchDocument<EndpointActionDto> patchDocument)
        {

            var actionInDb = await _endpointRuleRepository.FindAction(id);

            var updDto = _mapper.Map<EndpointActionDto>(actionInDb);

            patchDocument.ApplyTo(updDto, ModelState);

            _mapper.Map(updDto, actionInDb);
            UpdateAction(actionInDb);
            await _endpointRuleRepository.UpdateActionAsync(actionInDb);
            var updated = await _endpointRuleRepository.FindAction(id);
            return Ok(updated);
        }

        [HttpDelete]
        public async Task<ActionResult<EndpointRuleEntity>> Delete([FromBody] List<Guid> ids)
        {

            await _endpointRuleRepository.RemoveActionAsync(ids);
            return Ok();
        }


        [HttpPost("order")]
        public async Task<ActionResult<EndpointRuleDto>> UpdateActionsOrder(Guid ruleId, [FromBody] Dictionary<Guid, decimal> order)
        {

            await _endpointRuleRepository.UpdateActionsOrder(ruleId, order);
            return Ok();
        }

        private void UpdateAction(EndpointActionEntity actionEntity)
        {
            var mAction = _mapper.Map<MiddlerAction>(actionEntity);
            var action = _internalHelper.BuildConcreteActionInstance(mAction);
            if (action == null)
            {
                return;
            }

            if (action is ScriptingAction scriptAction)
            {
                actionEntity.Parameters["CompiledCode"] = scriptAction?.CompileScriptIfNeeded();
            }

        }

    }
}
