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
    public class LiteDBRuleRepository : IMiddlerRepository, IMiddlerStorage
    {
        public IObservable<MiddlerStorageEvent> EventObservable => this.EventSubject.AsObservable();

        private Subject<MiddlerStorageEvent> EventSubject { get; } = new Subject<MiddlerStorageEvent>();
        private LiteRepository Repository { get; }


        public LiteDBRuleRepository(string connectionString)
        {

            var con = new ConnectionString(connectionString);

            var fi = new FileInfo(con.Filename);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            Repository = new LiteRepository(con);
            Repository.Database.Mapper.Entity<MiddlerRuleDbModel>().Id(emp => emp.Id, true);
        }


        public List<MiddlerRule> ProvideRules()
        {
            return Repository.Fetch<MiddlerRuleDbModel>(model => model.Enabled).OrderBy(r => r.Order).Select(r => r.ToMiddlerRule()).ToList();
        }

        public async Task<List<MiddlerRuleDbModel>> GetAllAsync()
        {
            return Repository.Fetch<MiddlerRuleDbModel>(m => m != null).OrderBy(r => r.Order).ToList();
        }

        public async Task<MiddlerRuleDbModel> GetByIdAsync(Guid id)
        {
            return Repository.FirstOrDefault<MiddlerRuleDbModel>(emp => emp.Id == id);
        }

        public async Task AddAsync(MiddlerRuleDbModel employee)
        {

            if (String.IsNullOrWhiteSpace(employee.Name))
            {
                employee.Name = await GenerateRuleName();
            }
            Repository.Insert(employee);
            EventSubject.OnNext(new MiddlerStorageEvent(MiddlerStorageAction.Insert, employee));
        }

        public async Task RemoveAsync(Guid id)
        {
            Repository.Delete<MiddlerRuleDbModel>(id);
            EventSubject.OnNext(new MiddlerStorageEvent(MiddlerStorageAction.Insert, new MiddlerRuleDbModel() { Id = id }));
        }

        public async Task UpdateAsync(MiddlerRuleDbModel employee)
        {
            Repository.Update(employee);
            EventSubject.OnNext(new MiddlerStorageEvent(MiddlerStorageAction.Update, employee));
        }


        private async Task<string> GenerateRuleName()
        {

            int SplitNewRuleNames(string name)
            {
                if (name.Contains("("))
                {
                    var arr = name.Split("(");
                    var strnumb = arr[1].Trim(')');
                    return int.Parse(strnumb);
                }

                return 0;
            }


            var rules = await GetAllAsync();
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


    }
}
