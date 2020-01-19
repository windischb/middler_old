using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using middler.Common.Actions.UrlRedirect;
using middler.Common.Actions.UrlRewrite;
using middler.Core;
using middlerApp.Server.Attributes;
using middlerApp.Server.Controllers;
using middlerApp.Server.Data;
using middlerApp.Server.ExtensionMethods;
using middlerApp.Server.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using SignalARRR.Server.ExtensionMethods;

namespace middlerApp.Server
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
            });




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

            );

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddSingleton<WeatherForecastService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {


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
                    "[{RequestMethod}] {RequestPath} | {User} |-> {StatusCode} in {Elapsed:0.0000} ms";
            });






            app.UseWhen(context => context.IsAdminAreaRequest(), builder =>
            {
                builder.UseRouting();

                builder.UseStaticFiles();


                builder.UseEndpoints(endpoints =>
                {

                    endpoints.MapControllersWithAttribute<AdminControllerAttribute>();
                    endpoints.MapHub<UIHub>("/signalr/ui");
                    endpoints.MapBlazorHub();
                    endpoints.MapFallbackToPage("/_Host");

                });
            });


            app.UseWhen(context => !context.IsAdminAreaRequest(), builder =>
            {
                builder.UseRouting();

                builder.UseMiddler(map =>
                {
                    map.On("http", "*", "/admin/{**path}", actions => actions.RedirectTo("https://127.0.0.1:4444/{path}"));
                    map.On("http", "*", "{**path}", actions => actions.RedirectTo("https://{HOST}/{path}"));

                });

            });


            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            });



        }
    }
}
