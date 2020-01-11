using System;
using System.Diagnostics;
using System.IO;

namespace middler.WebHost.Helper
{
    public class PathHelper
    {

        public static string ContentPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName); // Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;


        public static string GetFullPath(string path, string basePath = null)
        {
            if (String.IsNullOrWhiteSpace(basePath))
            {
                basePath = ContentPath;
            }
            var p = Path.GetFullPath(Path.Combine(basePath, path));
            return p;
        }
    }
}
