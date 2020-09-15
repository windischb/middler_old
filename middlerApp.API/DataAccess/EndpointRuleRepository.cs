using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using middlerApp.API.IDP.Services;

namespace middlerApp.API.DataAccess
{
    public class EndpointRuleRepository
    {
        private readonly APPDbContext _appDbContext;
        public DataEventDispatcher EventDispatcher { get; }

        public EndpointRuleRepository(APPDbContext appDbContext, DataEventDispatcher eventDispatcher)
        {
            _appDbContext = appDbContext;
            EventDispatcher = eventDispatcher;
            //_appDbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }


        public async Task<IReadOnlyList<EndpointRuleEntity>> GetAllAsync()
        {
            return await _appDbContext.EndpointRules.Include(r => r.Actions).ToListAsync();
        }

        public async Task<EndpointRuleEntity> Find(Guid id)
        {
            return await _appDbContext.EndpointRules
                .Include(r => r.Actions)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<EndpointRuleEntity> GetByIdAsync(Guid id)
        {
            return await _appDbContext.EndpointRules.Include(endp => endp.Actions).FirstOrDefaultAsync(endp => endp.Id == id);
        }

        public async Task AddAsync(EndpointRuleEntity endpointRuleEntity)
        {
            if (String.IsNullOrWhiteSpace(endpointRuleEntity.Name))
            {
                endpointRuleEntity.Name = await GenerateRuleName();
            }

            if (endpointRuleEntity.Order == 0)
            {
                endpointRuleEntity.Order = _appDbContext.EndpointRules.Max(r => r.Order) + 10;
            }

            await _appDbContext.EndpointRules.AddAsync(endpointRuleEntity);
            await _appDbContext.SaveChangesAsync();
            EventDispatcher.DispatchCreatedEvent("EndpointRule", endpointRuleEntity);
        }

        public async Task RemoveAsync(Guid id)
        {
            var entity = await _appDbContext.EndpointRules.Include(endp => endp.Actions).FirstOrDefaultAsync(endp => endp.Id == id);
            _appDbContext.EndpointRules.Remove(entity);
            await _appDbContext.SaveChangesAsync();
            EventDispatcher.DispatchDeletedEvent("EndpointRule", id);
        }

        public async Task UpdateAsync(EndpointRuleEntity endpointRuleEntity)
        {
            //var entity = await _appDbContext.EndpointRules.Include(endp => endp.Actions).FirstOrDefaultAsync(endp => endp.Id == endpointRuleEntity.Id);
            _appDbContext.Entry(endpointRuleEntity).State = EntityState.Modified;
            await _appDbContext.SaveChangesAsync();
            EventDispatcher.DispatchUpdatedEvent("EndpointRule", endpointRuleEntity);
        }

        private async Task<string> GenerateRuleName()
        {

            int SplitNewRuleNames(string name)
            {
                if (name.Contains("("))
                {
                    var arr = name.Split('(');
                    var strnumb = arr[1].Trim(')');
                    return int.Parse(strnumb);
                }

                return 0;
            }


            var rules = await _appDbContext.EndpointRules.Where(r => r.Name.StartsWith("New Rule")).ToListAsync();
            var newRules = rules
                .Where(r => r.Name?.StartsWith("New Rule") == true)
                .Select(r => SplitNewRuleNames(r.Name))
                .OrderBy(n => n)
                .Distinct();

            int curr = 0;
            foreach (var newRule in newRules)
            {
                if (newRule == curr)
                {
                    curr++;
                }
                else
                {
                    break;
                }

            }

            return curr == 0 ? "New Rule" : $"New Rule({curr})";


        }




        public async Task<IReadOnlyList<EndpointActionEntity>> GetActionsForRuleAsync(Guid ruleId)
        {
            return await _appDbContext.EndpointActions.Where(a => a.EndpointRuleEntityId == ruleId).ToListAsync();
        }

        public async Task AddActionToRule(EndpointActionEntity actionEntity)
        {
            await _appDbContext.EndpointActions.AddAsync(actionEntity);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<EndpointActionEntity> FindAction(Guid id)
        {
            return await _appDbContext.EndpointActions.FindAsync(id);
        }

        public async Task UpdateActionAsync(EndpointActionEntity endpointActionEntity)
        {
            _appDbContext.Entry(endpointActionEntity).State = EntityState.Modified;
            await _appDbContext.SaveChangesAsync();
        }

        public async Task UpdateRulesOrder( Dictionary<Guid, decimal> order)
        {
            var rules = await _appDbContext.EndpointRules.AsTracking().ToListAsync();
            rules.ForEach(rule =>
            {
                if (order.ContainsKey(rule.Id))
                {
                    rule.Order = order[rule.Id];
                }
            });

            await _appDbContext.SaveChangesAsync();
        }

        public async Task UpdateActionsOrder(Guid ruleId, Dictionary<Guid, decimal> order)
        {
            var actions = await _appDbContext.EndpointActions.AsTracking().Where(act => act.EndpointRuleEntityId == ruleId).ToListAsync();
            actions.ForEach(act =>
            {
                if (order.ContainsKey(act.Id))
                {
                    act.Order = order[act.Id];
                }
            });

            await _appDbContext.SaveChangesAsync();
        }

        public async Task RemoveActionAsync(List<Guid> ids)
        {
            var actions = await _appDbContext.EndpointActions.Where(act => ids.Contains(act.Id)).ToListAsync();
            _appDbContext.EndpointActions.RemoveRange(actions);
        }
    }
}
