using AutoMapper;
using Newtonsoft.Json.Linq;

namespace middlerApp.Data.MapperProfiles.Formatters
{
    public class StringToJObjectFormatter : IValueConverter<string, JObject>
    {
        public JObject Convert(string sourceMember, ResolutionContext context)
        {
            return Converter.Json.ToJObject(sourceMember);
        }
    }
}