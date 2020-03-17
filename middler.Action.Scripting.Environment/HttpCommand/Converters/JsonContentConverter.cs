using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JsonSerializer = Utf8Json.JsonSerializer;

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

        public async Task ConvertToStream(object value, Stream stream)
        {
            await using var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true);

            await JsonSerializer.SerializeAsync(stream, value);
            
        }
    }
}
