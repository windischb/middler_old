using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace middlerApp.API.MapperProfiles
{
    public static class MappingHelper
    {
        internal static List<string> Split(string value)
        {
            return Split(value, ";");
        }
        internal static List<string> Split(string value, string delimiter)
        {
            return value?.Split(delimiter).Select(p => p.Trim()).Where(s => !String.IsNullOrWhiteSpace(s)).ToList() ?? new List<string>();
        }
    }
}
