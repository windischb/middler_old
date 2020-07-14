using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using middler.Common.SharedModels.Models;

namespace middlerApp.Data
{
    public class EndpointRuleRepository
    {
        private readonly MiddlerDbContext _middlerDbContext;

        public EndpointRuleRepository(MiddlerDbContext middlerDbContext)
        {
            _middlerDbContext = middlerDbContext;
            //_middlerDbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }


        public async Task<IReadOnlyList<EndpointRuleEntity>> GetAllAsync()
        {
            return await _middlerDbContext.EndpointRules.Include(r => r.Actions).ToListAsync();
        }

        public async Task<EndpointRuleEntity> Find(Guid id)
        {
            return await _middlerDbContext.EndpointRules.FindAsync(id);
        }

        public async Task<EndpointRuleEntity> GetByIdAsync(Guid id)
        {
            return await _middlerDbContext.EndpointRules.Include(endp => endp.Actions).FirstOrDefaultAsync(endp => endp.Id == id);
        }

        public async Task AddAsync(EndpointRuleEntity endpointRuleEntity)
        {
            if (String.IsNullOrWhiteSpace(endpointRuleEntity.Name))
            {
                endpointRuleEntity.Name = await GenerateRuleName();
            }

            if (endpointRuleEntity.Order == 0)
            {
                endpointRuleEntity.Order = _middlerDbContext.EndpointRules.Max(r => r.Order) + 10;
            }

            await _middlerDbContext.EndpointRules.AddAsync(endpointRuleEntity);
            await _middlerDbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            var entity = await _middlerDbContext.EndpointRules.Include(endp => endp.Actions).FirstOrDefaultAsync(endp => endp.Id == id);
            _middlerDbContext.EndpointRules.Remove(entity);
            await _middlerDbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(EndpointRuleEntity endpointRuleEntity)
        {
            //var entity = await _middlerDbContext.EndpointRules.Include(endp => endp.Actions).FirstOrDefaultAsync(endp => endp.Id == endpointRuleEntity.Id);
            _middlerDbContext.Entry(endpointRuleEntity).State = EntityState.Modified;
            await _middlerDbContext.SaveChangesAsync();
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


            var rules = await _middlerDbContext.EndpointRules.Where(r => r.Name.StartsWith("New Rule")).ToListAsync();
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
            return await _middlerDbContext.EndpointActions.Where(a => a.EndpointRuleEntityId == ruleId).ToListAsync();
        }

        public async Task AddActionToRule(EndpointActionEntity actionEntity)
        {
            await _middlerDbContext.EndpointActions.AddAsync(actionEntity);
            await _middlerDbContext.SaveChangesAsync();
        }

        public async Task<EndpointActionEntity> FindAction(Guid id)
        {
            return await _middlerDbContext.EndpointActions.FindAsync(id);
        }

        public async Task UpdateActionAsync(EndpointActionEntity endpointActionEntity)
        {
            _middlerDbContext.Entry(endpointActionEntity).State = EntityState.Modified;
            await _middlerDbContext.SaveChangesAsync();
        }

        public async Task UpdateRulesOrder( Dictionary<Guid, decimal> order)
        {
            var rules = await _middlerDbContext.EndpointRules.AsTracking().ToListAsync();
            rules.ForEach(rule =>
            {
                if (order.ContainsKey(rule.Id))
                {
                    rule.Order = order[rule.Id];
                }
            });

            await _middlerDbContext.SaveChangesAsync();
        }

        public async Task UpdateActionsOrder(Guid ruleId, Dictionary<Guid, decimal> order)
        {
            var actions = await _middlerDbContext.EndpointActions.AsTracking().Where(act => act.EndpointRuleEntityId == ruleId).ToListAsync();
            actions.ForEach(act =>
            {
                if (order.ContainsKey(act.Id))
                {
                    act.Order = order[act.Id];
                }
            });

            await _middlerDbContext.SaveChangesAsync();
        }

        public async Task RemoveActionAsync(List<Guid> ids)
        {
            var actions = await _middlerDbContext.EndpointActions.Where(act => ids.Contains(act.Id)).ToListAsync();
            _middlerDbContext.EndpointActions.RemoveRange(actions);
        }
    }
}
