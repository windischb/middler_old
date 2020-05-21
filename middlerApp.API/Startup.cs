using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using middler.Action.Scripting;
using middler.Action.Scripting.Powershell;
using middler.Common.Actions.UrlRedirect;
using middler.Common.Actions.UrlRewrite;
using middler.Core;
using middler.Storage.LiteDB;
using middlerApp.API.Attributes;
using middlerApp.API.ExtensionMethods;
using middlerApp.API.Helper;
using middlerApp.API.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using SignalARRR.Server.ExtensionMethods;
using middler.Variables.LiteDB;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

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

            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options =>
                {
                    options.Instance = "https://login.microsoftonline.com";
                    options.Domain = "winbe.onmicrosoft.com";
                    options.TenantId = "38b3384d-944e-403f-a649-e7990d0a69f9";
                    options.ClientId = "72d40c69-28f3-4e94-9c67-a474d722955c";
                    options.CallbackPath = "/signin-oidc";
                    options.SignedOutCallbackPath = "/signout-callback-oidc";
                });

            services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            {
                options.Authority = options.Authority + "/v2.0/";         // Microsoft identity platform

                options.TokenValidationParameters.ValidateIssuer = false; // accept several tenants (here simplified)
            });

            services.AddControllers(options =>
            {


            }).AddNewtonsoftJson(options =>
            {

                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.Converters.Add(new PSObjectJsonConverter());
                options.SerializerSettings.Converters.Add(new DecimalJsonConverter());
            });

            services.AddResponseCompression();
            services.AddSpaStaticFiles(conf => conf.RootPath = PathHelper.GetFullPath(sConfig.AdminSettings.WebRoot));



            services.AddSignalR().AddNewtonsoftJsonProtocol(options =>
            {
                options.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver();
                options.PayloadSerializerSettings.Converters.Add(new StringEnumConverter());
            });
            services.AddSignalARRR();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddMiddler(options =>
                options
                    .AddUrlRedirectAction()
                    .AddUrlRewriteAction()
                    .AddScriptingAction()

            );


            services.AddNamedMiddlerRepo("litedb", sp =>
            {
                var path = PathHelper.GetFullPath(sConfig.EndpointRulesSettings.DbFilePath);
                return new LiteDBRuleRepository($"Filename={path}");
            });

            services.AddSingleton(sp =>
            {
                var path = PathHelper.GetFullPath(sConfig.GlobalVariablesSettings.DbFilePath);
                return new VariableStore($"Filename={path}");
                //StoreConfig b = new StoreConfigBuilder().UseRootPath(PathHelper.GetFullPath(sConfig.GlobalVariablesSettings.RootPath));
                //return new VariablesStore(b);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {


            app.UseResponseCompression();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }


           

            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
                options.MessageTemplate =
                    "[{RequestMethod}] {RequestPath} | {User} | {StatusCode} in {Elapsed:0.0000} ms";
            });

            

            app.UseWhen(context => context.IsAdminAreaRequest(), builder =>
            {



                builder.UseRouting();

                builder.UseAuthentication();
                builder.UseAuthorization();

                builder.UseEndpoints(endpoints =>
                {

                    endpoints.MapControllersWithAttribute<AdminControllerAttribute>();
                    endpoints.MapHub<UIHub>("/signalr/ui");

                });


                //builder.UseDefaultFiles();
                builder.UseStaticFiles(new StaticFileOptions()
                {
                    OnPrepareResponse = ctx =>
                    {
                        if (ctx.Context.Request.Path.ToString() == "/index.html")
                        {
                            var headers = ctx.Context.Response.GetTypedHeaders();
                            headers.CacheControl = new CacheControlHeaderValue
                            {
                                Public = true,
                                MaxAge = TimeSpan.FromDays(0)
                            };
                        }
                    }
                });

                builder.UseSpa(spa =>
                {

                });

            });


            app.UseWhen(context => !context.IsAdminAreaRequest(), builder =>
            {
                builder.UseRouting();
                builder.UseAuthentication();
                builder.UseAuthorization();

                builder.UseMiddler(map => { map.AddNamedRepo("litedb"); });

            });


            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            });



        }
    }
}
