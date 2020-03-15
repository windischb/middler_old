using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace middler.Common.StreamHelper
{
    public static class StreamExtensions
    {
        private static async Task<List<Model>> DeserializeOptimizedFromStreamCallAsync(CancellationToken cancellationToken)
        {
            
                var stream = await response.Content.ReadAsStreamAsync();

                if (response.IsSuccessStatusCode)
                    return DeserializeJsonFromStream<List<Model>>(stream);

                var content = await StreamToStringAsync(stream);
                throw new ApiException
                {
                    StatusCode = (int)response.StatusCode,
                    Content = content
                };
            
        }

        private static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
                return default(T);

            using (var sr = new StreamReader(stream))
            using (var jtr = new JsonTextReader(sr))
            {
                var js = new JsonSerializer();
                var searchResult = js.Deserialize<T>(jtr);
                return searchResult;
            }
        }
    }
}
