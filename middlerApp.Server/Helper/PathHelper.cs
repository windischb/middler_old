using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace middlerApp.Server.Helper
{
    public class PathHelper
    {

        public static string ContentPath = IsDevelopment() ? Directory.GetCurrentDirectory() : Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName);


        public static string GetFullPath(string path, string basePath = null)
        {

            if (String.IsNullOrWhiteSpace(basePath))
            {
                basePath = ContentPath;
            }
            var p = Path.GetFullPath(Path.Combine(basePath, path));
            return p;
        }

        private static bool IsDevelopment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development;
        }
    }
}
