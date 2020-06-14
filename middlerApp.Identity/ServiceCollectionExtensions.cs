using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace middlerApp.Identity
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMiddlerIdentityServer(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> dbContextOptionsBuilder)
        {
            serviceCollection.AddIdentityServer(options => { })
                .AddTestUsers(TestUsers.Users)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = dbContextOptionsBuilder;
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = dbContextOptionsBuilder;
                });
        }
    }
}
