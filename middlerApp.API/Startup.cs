using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
using middlerApp.API.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
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

            services.AddControllers(options =>
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

            services.AddResponseCompression();
            services.AddSpaStaticFiles(conf => conf.RootPath = PathHelper.GetFullPath(sConfig.AdminSettings.WebRoot));



            services.AddSignalR().AddNewtonsoftJsonProtocol(options =>
            {
                options.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver();
                options.PayloadSerializerSettings.Converters.Add(new StringEnumConverter());
                options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.AddSignalARRR();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddMiddler(options =>
                options
                    .AddUrlRedirectAction()
                    .AddUrlRewriteAction()
                    .AddScriptingAction()

            );

            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = MiddlerApp"));

            //services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Filename=MyDatabase.db"), ServiceLifetime.Scoped);

            services.AddScoped<EndpointRuleRepository>();

            services.AddMiddlerRepo<EFCoreMiddlerRepository>(ServiceLifetime.Scoped);

            services.AddScoped<IVariablesRepository, VariablesRepository>();


            services.AddMiddlerIdentityServer(opt => opt.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = MiddlerApp", sql => sql.MigrationsAssembly(this.GetType().Assembly.FullName)));

            //services.AddNamedMiddlerRepo("litedb", sp =>
            //{
            //    var path = PathHelper.GetFullPath(sConfig.EndpointRulesSettings.DbFilePath);
            //    return new LiteDBRuleRepository($"Filename={path}");
            //});

            //services.AddSingleton(sp =>
            //{
            //    var path = PathHelper.GetFullPath(sConfig.GlobalVariablesSettings.DbFilePath);
            //    return new VariableStore($"Filename={path}");
            //    StoreConfig b = new StoreConfigBuilder().UseRootPath(PathHelper.GetFullPath(sConfig.GlobalVariablesSettings.RootPath));
            //    return new VariablesStore(b);
            //});
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

            app.UseMiddlerIdentityServer();

            app.UseWhen(context => context.IsAdminAreaRequest(), builder =>
            {



                builder.UseRouting();

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
                builder.UseMiddler(map =>
                {
                    map.AddRepo<EFCoreMiddlerRepository>();
                    //map.AddNamedRepo("litedb");
                });

            });


            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            });



        }
    }
}
