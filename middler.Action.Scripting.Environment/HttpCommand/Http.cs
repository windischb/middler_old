using System;

namespace middler.Scripting.HttpCommand
{


    public class Http
    {

        public HttpRequestBuilder Client(Action<HttpOptionsBuilder> options)
        {
            var builder = new HttpOptionsBuilder();
            options?.Invoke(builder);
            return new HttpRequestBuilder(builder);
        }

        public HttpRequestBuilder Client(HttpHandlerOptions options)
        {
            return new HttpRequestBuilder(options);
        }

        public HttpRequestBuilder Client()
        {
            return new HttpRequestBuilder();
        }

        public HttpRequestBuilder Client(string url)
        {
            var builder = new HttpOptionsBuilder().UseBaseUrl(url);
            return new HttpRequestBuilder(builder);
        }

    }
}
