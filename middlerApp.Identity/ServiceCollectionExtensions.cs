using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using middlerApp.Identity.Models;

namespace middlerApp.Identity
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMiddlerIdentityServer(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> dbContextOptionsBuilder)
        {

            serviceCollection.AddDbContext<ApplicationDbContext>(dbContextOptionsBuilder);

            serviceCollection.AddIdentity<MIdentityUser, IdentityRole>(options =>
                {

                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            serviceCollection.AddIdentityServer(options => { })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = dbContextOptionsBuilder;
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = dbContextOptionsBuilder;
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 3600;
                });
        }
    }
}
