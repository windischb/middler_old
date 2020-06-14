using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace middlerApp.Data
{
    public class MiddlerDbContext : DbContext
    {
        public DbSet<EndpointRuleEntity> EndpointRules { get; set; }
        public DbSet<EndpointActionEntity> EndpointActions { get; set; }

        public DbSet<TreeNode> Variables { get; set; }

        public MiddlerDbContext(DbContextOptions<MiddlerDbContext> options): base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<EndpointActionEntity>()
                .Property(p => p.Parameters)
                .HasConversion(
                    v => v.ToString(),
                    str => JObject.Parse(str)
                    );

            modelBuilder
                .Entity<TreeNode>(e => e.Ignore(p => p.Children));

            modelBuilder
                .Entity<TreeNode>()
                .Property(p => p.Content)
                .HasConversion(
                    v => ToJsonString(v),
                    str =>ToJToken(str)
                );


        }

        private string ToJsonString(JToken jToken)
        {
            var jsonString = Converter.Json.ToJson(jToken);
            return jsonString;
        }

        private JToken ToJToken(string jsonString)
        {

            var jToken = Converter.Json.ToJToken(jsonString);
            return jToken;
        }
    }
}
