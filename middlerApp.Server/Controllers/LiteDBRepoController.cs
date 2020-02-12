using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using middler.Common.Actions.UrlRedirect;
using middler.Common.Storage;
using middler.Core.ExtensionMethods;
using middler.Hosting.Models;
using middlerApp.Server.Attributes;
using NamedServices.Microsoft.Extensions.DependencyInjection;

namespace middlerApp.Server.Controllers
{
    [ApiController]
    [Route("api/repo/litedb")]
    [AdminController]
    public class LiteDBRepoController: Controller
    {
        private readonly IMapper _mapper;
        private IMiddlerStorage Repo { get; }

        public LiteDBRepoController(IServiceProvider serviceProvider, IMapper mapper)
        {
            _mapper = mapper;
            Repo = serviceProvider.GetNamedService<IMiddlerStorage>("litedb");

            var r = new MiddlerRuleDbModel();
            r.Name = "Redirect All HTTP to HTTPS";

            var found = Repo.GetAllAsync().GetAwaiter().GetResult().FirstOrDefault(rule => rule.Name == r.Name);
            if (found == null) {
                r.Scheme = new List<string> { "http" };
                r.Hostname = "*";
                r.Path = "{**Path}";
                var act = new UrlRedirectAction();
                act.Parameters.RedirectTo = "https://{HOST}/{path}";
                
                r.Actions.Add(act.ToBasicMiddlerAction());
                r.Enabled = true;

                Repo.AddAsync(r);
            }

        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MiddlerRuleDto>>> GetAll()
        {
            var rules = await Repo.GetAllAsync();

            return Ok(_mapper.Map<IEnumerable<MiddlerRuleDto>>(rules));
        }


        [HttpPost]
        public async Task<ActionResult> Add([FromBody]CreateMiddlerRuleDto rule) {
            var dbModel = _mapper.Map<MiddlerRuleDbModel>(rule);


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
            await Repo.UpdateAsync(dbModel);
            var updated = await Repo.GetByIdAsync(id);
            return Ok(_mapper.Map<MiddlerRuleDto>(updated));
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<MiddlerRuleDto>> PartialUpdate(Guid id, [FromBody]JsonPatchDocument<UpdateMiddlerRuleDto> patchDocument) {

            var ruleInDb = await Repo.GetByIdAsync(id);

            var updDto = _mapper.Map<UpdateMiddlerRuleDto>(ruleInDb);
            patchDocument.ApplyTo(updDto, ModelState);

            _mapper.Map(updDto, ruleInDb);

            await Repo.UpdateAsync(ruleInDb);
            var updated = await Repo.GetByIdAsync(id);
            return Ok(_mapper.Map<MiddlerRuleDto>(updated));
        }
        
    }
}
