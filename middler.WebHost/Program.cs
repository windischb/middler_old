using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BMI.Benutzerverwaltung.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using middler.WebHost.Helper;
using Serilog;
using Serilog.Events;

namespace middler.WebHost
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
            return new HostBuilder()
                .UseSerilog()
                .ConfigureHostConfiguration(BuildHostConfiguration)
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .UseContentRoot(PathHelper.ContentPath)
                    .UseKestrel(ConfigureKestrel)
                    .UseStartup<Startup>()
                );
        }

        private static void BuildHostConfiguration(IConfigurationBuilder config)
        {
            if (!File.Exists(PathHelper.GetFullPath("configuration.json")))
            {
                var json = Converter.Json.ToJson(new StartUpConfiguration(), true);
                File.WriteAllText(PathHelper.GetFullPath("configuration.json"), json);
            }
            config.AddJsonFile(PathHelper.GetFullPath("configuration.json"), optional: false);
        }

        private static void ConfigureKestrel(WebHostBuilderContext context, KestrelServerOptions serverOptions)
        {

            var config = context.Configuration.Get<StartUpConfiguration>();
            var listenIp = IPAddress.Parse(config.ListenIP);

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


        }


    }
}
