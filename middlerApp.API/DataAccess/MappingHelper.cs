using System;
using System.Collections.Generic;
using System.Linq;

namespace middlerApp.API.DataAccess
{
    internal static class MappingHelper
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
