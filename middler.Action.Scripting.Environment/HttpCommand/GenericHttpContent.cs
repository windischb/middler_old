using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using middler.Scripting.ExtensionMethods;
using middler.Scripting.HttpCommand.Converters;
using Newtonsoft.Json.Linq;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using Utf8Json;

namespace middler.Scripting.HttpCommand
{
    public class GenericHttpContent: IDisposable
    {

        private readonly HttpContent _httpContent;

        public ExpandoObject Headers => _httpContent.Headers.ToExpandoObject();
        public Task<string> ReadAsStringAsync() => _httpContent.ReadAsStringAsync();

        public Task<byte[]> ReadAsByteArrayAsync() => _httpContent.ReadAsByteArrayAsync();

        public Task<Stream> ReadAsStreamAsync() => _httpContent.ReadAsStreamAsync();

        public Task CopyToAsync(Stream stream, TransportContext context) => _httpContent.CopyToAsync(stream, context);

        public Task CopyToAsync(Stream stream) => _httpContent.CopyToAsync(stream);

        public Task LoadIntoBufferAsync() => _httpContent.LoadIntoBufferAsync();

        public Task LoadIntoBufferAsync(long maxBufferSize) => _httpContent.LoadIntoBufferAsync(maxBufferSize);


        public GenericHttpContent(HttpContent httpContent)
        {
            _httpContent = httpContent;
        }

        public void Dispose()
        {
            _httpContent?.Dispose();
        }
        
        
        public object ToObject()
        {
            return ToObject<ExpandoObject>();
        }

        public object ToArray()
        {
            return ToObject<ExpandoObject[]>();
        }


        public object ToObject<T>()
        {
            switch (_httpContent.Headers?.ContentType?.MediaType)
            {
                case "application/json":
                {
                    return new JsonContentConverter().ConvertToObject<T>(ReadAsStreamAsync().WaitAndUnwrapException());
                }
            }    

            throw new NotImplementedException();
        }

      
        //public object AsJsonToObject()
        //{
        //    return AsJsonToObject<ExpandoObject>();
        //}

        //public object AsJsonToArray()
        //{
        //    return AsJsonToObject<ExpandoObject[]>();
        //}

        //public T AsJsonToObject<T>()
        //{
        //    return JsonSerializer.Deserialize<T>(ReadAsStreamAsync().WaitAndUnwrapException());
        //}

    }
}