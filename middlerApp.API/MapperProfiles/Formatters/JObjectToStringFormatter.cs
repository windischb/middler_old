using AutoMapper;
using middlerApp.API.Helper;
using Newtonsoft.Json.Linq;

namespace middlerApp.API.MapperProfiles.Formatters
{
    public class JObjectToStringFormatter : IValueConverter<JObject, string>
    {

        public string Convert(JObject sourceMember, ResolutionContext context)
        {
            return Converter.Json.ToJson(sourceMember);
        }
    }
}