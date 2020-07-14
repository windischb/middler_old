using AutoMapper;
using middlerApp.API.Helper;
using Newtonsoft.Json.Linq;

namespace middlerApp.API.MapperProfiles.Formatters
{
    public class StringToJObjectFormatter : IValueConverter<string, JObject>
    {
        public JObject Convert(string sourceMember, ResolutionContext context)
        {
            return Converter.Json.ToJObject(sourceMember);
        }
    }
}