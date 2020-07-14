using System;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using middler.IDP.DbContexts;
using middler.IDP.Services;
using middler.IDP.Storage.Stores;

namespace middler.IDP
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMiddlerIdentityServer(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptionsBuilder)
        {

            services.AddDbContext<IdentityDbContext>(dbContextOptionsBuilder);

            
            services.AddIdentityServer(options => { })
                .AddClientStore<ClientStore>()
                .AddDeviceFlowStore<DeviceFlowStore>()
                .AddResourceStore<ResourceStore>()
                .AddPersistedGrantStore<PersistedGrantStore>()
                .AddCorsPolicyService<MCorsPolicyService>();

            
            //services.AddTransient<IUserConsentStore, UserConsentStore>();
            

        }
    }
}
