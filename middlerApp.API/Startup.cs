using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using middler.Action.Scripting;
using middler.Action.Scripting.Powershell;
using middler.Common.Actions.UrlRedirect;
using middler.Common.Actions.UrlRewrite;
using middler.Core;
using middler.Storage.LiteDB;
using middlerApp.API.Attributes;
using middlerApp.API.ExtensionMethods;
using middlerApp.API.Helper;
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

            services.AddControllers(options =>
            {

            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.Converters.Add(new PSObjectJsonConverter());
            });

            services.AddResponseCompression();




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
            services.AddNamedMiddlerRepo("litedb", sp => new LiteDBRuleRepository("Filename=rules.db"));

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
               

                builder.UseStaticFiles();
                builder.UseRouting();

                builder.UseEndpoints(endpoints =>
                {

                    endpoints.MapControllersWithAttribute<AdminControllerAttribute>();
                    endpoints.MapHub<UIHub>("/signalr/ui");

                });
            });


            app.UseWhen(context => !context.IsAdminAreaRequest(), builder =>
            {
                builder.UseRouting();

                builder.UseMiddler(map => { map.AddNamedRepo("litedb"); });

            });


            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            });



        }
    }
}
