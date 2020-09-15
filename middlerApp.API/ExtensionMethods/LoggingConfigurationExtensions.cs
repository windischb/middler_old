using Microsoft.AspNetCore.Builder;
using middlerApp.API.Helper;
using Serilog;

namespace middlerApp.API.ExtensionMethods
{
    public static class LoggingConfigurationExtensions
    {
        public static IApplicationBuilder AddLogging(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
                options.MessageTemplate =
                    "[{RequestMethod}] {RequestPath} | {User} | {StatusCode} in {Elapsed:0.0000} ms";
            });

            return app;
        }
    }
}
