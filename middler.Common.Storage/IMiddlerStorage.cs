using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace middler.Common.Storage
{
    public interface IMiddlerStorage
    {
        Task<List<MiddlerRuleDbModel>> GetAllAsync();
        Task<MiddlerRuleDbModel> GetByIdAsync(Guid id);
        Task AddAsync(MiddlerRuleDbModel employee);
        Task RemoveAsync(Guid id);
        Task UpdateAsync(MiddlerRuleDbModel employee);

        IObservable<MiddlerStorageEvent> EventObservable { get; }

    }
}
