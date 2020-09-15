using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using middler.Action.Scripting;
using middler.Action.Scripting.Powershell;
using middler.Common.Actions.UrlRedirect;
using middler.Common.Actions.UrlRewrite;
using middler.Common.SharedModels.Interfaces;
using middler.Core;
using middlerApp.API.Attributes;
using middlerApp.API.DataAccess;
using middlerApp.API.ExtensionMethods;
using middlerApp.API.Helper;
using middlerApp.API.IDP;
using middlerApp.API.IDP.Services;
using middlerApp.API.JsonConverters;
using middlerApp.API.Middleware;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using SignalARRR.Server;
using SignalARRR.Server.ExtensionMethods;


namespace middlerApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            var sConfig = Configuration.Get<StartUpConfiguration>();
            sConfig.SetDefaultSettings();

           
            services.AddMvc(options =>
            {


            }).AddNewtonsoftJson(options =>
            {

                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.Converters.Add(new PSObjectJsonConverter());
                options.SerializerSettings.Converters.Add(new DecimalJsonConverter());
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            //services.AddSingleton<PartialViewResultExecutor>();

            services.AddResponseCompression();
            services.AddSpaStaticFiles();



            services.AddSignalR().AddNewtonsoftJsonProtocol(options =>
            {
                options.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver();
                options.PayloadSerializerSettings.Converters.Add(new StringEnumConverter());
                options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });


            //services.AddScoped<IAuthenticator, OAuthAuthenticator>();
            services.AddSignalARRR();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

           

            services.AddMiddlerIdentityServer(opt => opt.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = MiddlerApp", sql => sql.MigrationsAssembly(this.GetType().Assembly.FullName)));
            //services.AddAuthentication();

            //services.AddSingleton<ICorsPolicyService>((container) => {
            //    var logger = container.GetRequiredService<ILogger<DefaultCorsPolicyService>>();
            //    return new DefaultCorsPolicyService(logger)
            //    {
            //        AllowAll = true
                    
            //    };
            //});

            services.AddMiddler(options =>
                options
                    .AddUrlRedirectAction()
                    .AddUrlRewriteAction()
                    .AddScriptingAction()

            );

            services.AddDbContext<APPDbContext>(opt => opt.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = MiddlerApp"));

            

            services.AddScoped<EndpointRuleRepository>();

            services.AddMiddlerRepo<EFCoreMiddlerRepository>(ServiceLifetime.Scoped);

            services.AddScoped<IVariablesRepository, VariablesRepository>();


           

            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");


            services.Configure<ForwardedHeadersOptions>(options =>
            {
                //options.ForwardLimit = 4;
                //options.KnownProxies.Add(IPAddress.Parse("127.0.10.1"));
                //options.ForwardedForHeaderName = "X-Forwarded-For-My-Custom-Header-Name";
                options.ForwardedHeaders =  ForwardedHeaders.All;
            });


           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> _logger)
        {

            app.UseForwardedHeaders();
            app.UseResponseCompression();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseWhen(context => context.IsIdpAreaRequest(), ConfigureIDP);

            app.UseWhen(context => context.IsAdminAreaRequest(), ConfigureAdministration);

            app.UseWhen(context => !context.IsAdminAreaRequest() && !context.IsIdpAreaRequest(),ConfigureMiddler);

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            });



        }

      
        public void ConfigureIDP(IApplicationBuilder app)
        {
            app.AddLogging();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllersWithAttribute<IdPControllerAttribute>();

            });

            app.UseMiddlerSpaUI(Static.StartUpConfiguration.IdpSettings.WebRoot);
        }

        public void ConfigureAdministration(IApplicationBuilder app)
        {
            app.AddLogging();
            
            app.UseRouting();
            app.UseSignalARRRAccessTokenValidation();
            app.UseAuthentication();
            app.UseAuthorization();

            
            app.UseMiddleware<LogClaimsMiddleware>();


            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllersWithAttribute<AdminControllerAttribute>();
                endpoints.MapHub<UIHub>("/signalr/ui");

            });

            app.UseMiddlerSpaUI(Static.StartUpConfiguration.AdminSettings.WebRoot);
        }

        public void ConfigureMiddler(IApplicationBuilder app)
        {
            app.AddLogging();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddler(map =>
            {
                map.AddRepo<EFCoreMiddlerRepository>();
            });
        }
    }
}
