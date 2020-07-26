using System;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using middlerApp.API.IDP.Models;
using middlerApp.API.IDP.Services;
using middlerApp.API.IDP.Storage.Stores;

namespace middlerApp.API.IDP
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMiddlerIdentityServer(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptionsBuilder)
        {
            services.AddSingleton<DataEventDispatcher>();

            services.AddScoped<IPasswordHasher<MUser>, PasswordHasher<MUser>>();
            services.AddScoped<ILocalUserService, LocalUserService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IRolesService, RolesService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IApiResourcesService, ApiResourcesService>();
            services.AddScoped<IIdentityResourcesService, IdentityResourcesService>();


            services.AddDbContext<IDPDbContext>(dbContextOptionsBuilder);

            //services.AddScoped<IAuthorizationCodeStore, AuthorizationCodeStore>();
            services.AddScoped<IUserConsentStore, UserConsentStore>();

            services.AddIdentityServer(options => { })
                .AddClientStore<ClientStore>()
                .AddDeviceFlowStore<DeviceFlowStore>()
                .AddResourceStore<ResourceStore>()
                .AddPersistedGrantStore<PersistedGrantStore>()
                .AddCorsPolicyService<MCorsPolicyService>()
                .AddProfileService<LocalUserProfileService>();


            


        }
    }
}
