using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using middler.Core;
using middlerApp.API.Attributes;
using middlerApp.API.DataAccess;
using middlerApp.SharedModels;


namespace middlerApp.API.Controllers
{
    [ApiController]
    [Route("api/endpoint-rules")]
    [AdminController]
    public class EndpointRulesController: Controller
    {
        private readonly IMapper _mapper;
        private readonly InternalHelper _internalHelper;
        private readonly EndpointRuleRepository _endpointRuleRepository;

        public EndpointRulesController(IServiceProvider serviceProvider, IMapper mapper, InternalHelper internalHelper, EndpointRuleRepository endpointRuleRepository)
        {
            _mapper = mapper;
            _internalHelper = internalHelper;
            _endpointRuleRepository = endpointRuleRepository;

        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<EndpointRuleListDto>>> GetAll()
        {
            var rules = await _endpointRuleRepository.GetAllAsync();
            return Ok(_mapper.Map<List<EndpointRuleListDto>>(rules));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EndpointRuleDto>> Get(Guid id)
        {
            var rule = await _endpointRuleRepository.Find(id);
            return Ok(_mapper.Map<EndpointRuleDto>(rule));
        }


        [HttpPost]
        public async Task<ActionResult> Add([FromBody]EndpointRuleEntity rule)
        {

            await _endpointRuleRepository.AddAsync(rule);

            //var dbModel = _mapper.Map<MiddlerRuleDbModel>(rule);
            //UpdateActions(dbModel);
            //await Repo.AddAsync(dbModel);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _endpointRuleRepository.RemoveAsync(id);
            return Ok();
        }

        //[HttpPut("{id}")]
        //public async Task<ActionResult<EndpointRuleDto>> Update(Guid id, [FromBody]EndpointRuleEntity rule)
        //{
        //    var dbModel = _mapper.Map<MiddlerRuleDbModel>(rule);
        //    dbModel.Id = id;
        //    UpdateActions(dbModel);
        //    await Repo.UpdateAsync(dbModel);
        //    var updated = await Repo.GetByIdAsync(id);
        //    return Ok(ToDto(updated));
        //}

        [HttpPatch("{id}")]
        public async Task<ActionResult<EndpointRuleEntity>> PartialUpdate(Guid id, [FromBody]JsonPatchDocument<EndpointRuleDto> patchDocument) {

            var ruleInDb = await _endpointRuleRepository.Find(id);

            var updDto = _mapper.Map<EndpointRuleDto>(ruleInDb);
            
            patchDocument.ApplyTo(updDto, ModelState);

            _mapper.Map(updDto, ruleInDb);
            //UpdateActions(ruleInDb);
            await _endpointRuleRepository.UpdateAsync(ruleInDb);
            var updated = await _endpointRuleRepository.GetByIdAsync(id);
            return Ok(updated);
        }

        [HttpPost("order")]
        public async Task<ActionResult<EndpointRuleDto>> UpdateRulesOrder([FromBody] Dictionary<Guid, decimal> order)
        {

            await _endpointRuleRepository.UpdateRulesOrder(order);
            return Ok();
        }


    }
}
