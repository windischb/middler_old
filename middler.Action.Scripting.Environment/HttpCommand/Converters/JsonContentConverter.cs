using System.IO;
using System.Net.Http.Headers;
using Utf8Json;

namespace middler.Scripting.HttpCommand.Converters
{
    public class JsonContentConverter: ContentConverter
    {
        public override bool CanConvert(HttpContentHeaders contentHeaders)
        {
            return contentHeaders.ContentType.MediaType == "application/json";
        }

        public override T ConvertToObject<T>(Stream stream)
        {
            return JsonSerializer.Deserialize<T>(stream);
        }
    }
}
