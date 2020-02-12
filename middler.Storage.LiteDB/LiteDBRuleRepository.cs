using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using middler.Common;
using middler.Common.Interfaces;
using middler.Common.SharedModels.Models;
using middler.Common.Storage;

namespace middler.Storage.LiteDB
{
    public class LiteDBRuleRepository: IMiddlerRepository, IMiddlerStorage {
        public IObservable<MiddlerStorageEvent> EventObservable => this.EventSubject.AsObservable();

        private Subject<MiddlerStorageEvent> EventSubject { get; } = new Subject<MiddlerStorageEvent>();
        private LiteRepository Repository { get; }
        

        public LiteDBRuleRepository(string connectionString) {
            Repository = new LiteRepository(connectionString);
            Repository.Database.Mapper.Entity<MiddlerRuleDbModel>().Id(emp => emp.Id, true);
        }


        public List<MiddlerRule> ProvideRules()
        {
            return Repository.Fetch<MiddlerRuleDbModel>().Where(r => r.Enabled).Select(r => r.ToMiddlerRule()).ToList();
        }

        public async Task<List<MiddlerRuleDbModel>> GetAllAsync()
        {
            return Repository.Fetch<MiddlerRuleDbModel>();
        }

        public async Task<MiddlerRuleDbModel> GetByIdAsync(Guid id)
        {
            return Repository.FirstOrDefault<MiddlerRuleDbModel>(emp => emp.Id == id);
        }

        public async Task AddAsync(MiddlerRuleDbModel employee)
        {
            Repository.Insert(employee);
            EventSubject.OnNext(new MiddlerStorageEvent(MiddlerStorageAction.Insert, employee));
        }

        public async Task RemoveAsync(Guid id)
        {
            Repository.Delete<MiddlerRuleDbModel>(emp => emp.Id == id);
            EventSubject.OnNext(new MiddlerStorageEvent(MiddlerStorageAction.Insert, new MiddlerRuleDbModel(){Id = id}));
        }

        public async Task UpdateAsync(MiddlerRuleDbModel employee)
        {
            Repository.Update(employee);
            EventSubject.OnNext(new MiddlerStorageEvent(MiddlerStorageAction.Update, employee));
        }

        
    }
}
