using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using middler.Common.Actions.UrlRedirect;
using middler.Common.Actions.UrlRewrite;
using middler.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using SignalARRR.Server.ExtensionMethods;

namespace middler.WebHost
{
    public class Startup
    {
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

            services.AddRazorPages();
            services.AddServerSideBlazor();

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

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSerilogRequestLogging();


            app.UseMiddler(map =>
            {
                map.On("http", "*", "test/{**path}", actions => actions.RedirectTo("https://{HOST}/{path}"));
                map.On("http", "*", "{**path}", actions => actions.RewriteTo("rewritedÖ"));

            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<UIHub>("/signalr/ui");
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
