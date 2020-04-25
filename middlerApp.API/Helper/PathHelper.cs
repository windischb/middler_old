using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace middlerApp.API.Helper
{
    public class PathHelper
    {

        public static string ContentPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName);


        public static string GetFullPath(string path, string basePath = null)
        {
            if (String.IsNullOrWhiteSpace(basePath))
            {
                basePath = ContentPath;
            }
            var p = Path.GetFullPath(Path.Combine(basePath, path));
            return p;
        }

        public static string GetWebRoot()
        {
            if (IsDevelopment())
            {
                return GetFullPath("wwwroot", Directory.GetCurrentDirectory());
            }

            return GetFullPath("wwwroot");
        }

        private static bool IsDevelopment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development;
        }
    }
}
