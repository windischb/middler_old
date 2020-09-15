using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using middlerApp.API.Helper;
using Serilog;
using Serilog.Events;

namespace middlerApp.API
{
    
    public class Program
    {

       

        public static int Main(string[] args)
        {

            try
            {
                ConfigureLogging();

                Log.Information("Starting host");
                CreateWebHostBuilder(args).Build().Run();
                //CreateAdminHost(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }



        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .MinimumLevel.Override("IdentityServer4", LogEventLevel.Verbose)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args)
        {

            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration(BuildHostConfiguration)
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                        .UseContentRoot(PathHelper.ContentPath)
                        .UseWebRoot(PathHelper.GetFullPath(Static.StartUpConfiguration.AdminSettings.WebRoot))
                        .UseKestrel(ConfigureKestrel)
                        .UseStartup<Startup>()
                );
        }

       
        private static void BuildHostConfiguration(HostBuilderContext context, IConfigurationBuilder config)
        {

            var env = context.HostingEnvironment;
            config.AddJsonFile(PathHelper.GetFullPath("configuration.json"), optional: true);
            config.AddEnvironmentVariables();

            //var conf = config.Build().Get<StartUpConfiguration>();
            
        }

        private static void ConfigureKestrel(WebHostBuilderContext context, KestrelServerOptions serverOptions)
        {
            Log.Debug("ConfigureKestrel");
            var config = context.Configuration.Get<StartUpConfiguration>();
            config.SetDefaultSettings();
            
            var listenIp = IPAddress.Parse(config.ListeningIP);

            if (config.HttpPort.HasValue && config.HttpPort.Value != 0)
            {
                serverOptions.Listen(listenIp, config.HttpPort.Value, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                });
            }

            if (config.HttpsPort.HasValue && config.HttpsPort.Value != 0)
            {
                var certPath = PathHelper.GetFullPath(config.HttpsCertPath);
                Log.Debug(certPath);
                serverOptions.Listen(listenIp, config.HttpsPort.Value, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    listenOptions.UseHttps(PathHelper.GetFullPath(config.HttpsCertPath), config.HttpsCertPassword);
                });
            }

            var adminListenIp = IPAddress.Parse(config.AdminSettings.ListeningIP);
            serverOptions.Listen(adminListenIp, config.AdminSettings.HttpsPort, options =>
            {
                options.Protocols = HttpProtocols.Http1AndHttp2;
                options.UseHttps(PathHelper.GetFullPath(config.AdminSettings.HttpsCertPath),
                    config.AdminSettings.HttpsCertPassword);
            });

            var idpListenIp = IPAddress.Parse(config.IdpSettings.ListeningIP);
            serverOptions.Listen(idpListenIp, config.IdpSettings.HttpsPort, options =>
            {
                options.Protocols = HttpProtocols.Http1AndHttp2;
                
                options.UseHttps(PathHelper.GetFullPath(config.IdpSettings.HttpsCertPath), config.IdpSettings.HttpsCertPassword);
            });

        }


        
    }

    
}
