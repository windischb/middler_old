using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using middlerApp.API.Helper;

namespace middlerApp.API
{
    public static class middlerSpaExtensions
    {
        public static void UseMiddlerSpaUI(this IApplicationBuilder app, string path)
        {

   

            var stfOptions = new StaticFileOptions()
            {
                RequestPath = "",
                FileProvider = new PhysicalFileProvider(PathHelper.GetFullPath(path)),
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
            };


            app.UseDefaultFiles();
            app.UseStaticFiles(stfOptions);

            app.UseSpa(spa =>
            {
                spa.Options.DefaultPageStaticFileOptions = stfOptions;
            });
        }
    }
}
