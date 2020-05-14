using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace middlerApp.API.ExtensionMethods
{
    public static class ListExtensions
    {
        public static List<T> EnsureList<T>(this List<T> list)
        {
            return list ?? new List<T>();
        }
    }
}
