using System;
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
                CreateHost(args).Build().Run();
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
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }

        public static IHostBuilder CreateHost(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                
                .ConfigureAppConfiguration(BuildHostConfiguration)

                .ConfigureWebHostDefaults(webBuilder => webBuilder
                        .UseContentRoot(PathHelper.ContentPath)
                        .UseWebRoot(PathHelper.GetFullPath("wwwroot"))
                        .UseKestrel(ConfigureKestrel)
                        .UseStartup<Startup>()
                );
        }

       
        private static void BuildHostConfiguration(HostBuilderContext context, IConfigurationBuilder config)
        {

            var env = context.HostingEnvironment;

            config.AddYamlFile(PathHelper.GetFullPath("configuration.yml"), optional: true);
            config.AddJsonFile(PathHelper.GetFullPath("configuration.json"), optional: true);
            config.AddYamlFile(PathHelper.GetFullPath($"configuration.{env.EnvironmentName}.yml"), optional: true);
            config.AddJsonFile(PathHelper.GetFullPath($"configuration.{env.EnvironmentName}.json"), optional: true);

        }

        private static void ConfigureKestrel(WebHostBuilderContext context, KestrelServerOptions serverOptions)
        {

            var config = context.Configuration.Get<StartUpConfiguration>();
            config.SetAdminSettings();
            
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
                options.UseHttps(PathHelper.GetFullPath(config.AdminSettings.HttpsCertPath), config.AdminSettings.HttpsCertPassword);
            });

        }

    }

    
}
