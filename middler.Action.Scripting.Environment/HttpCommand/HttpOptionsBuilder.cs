using System;
using System.Net;

namespace middler.Scripting.HttpCommand
{
    public class HttpOptionsBuilder
    {
        private readonly HttpHandlerOptions _httpHandlerOptions = new HttpHandlerOptions();

        public HttpOptionsBuilder()
        {

        }

        public HttpOptionsBuilder UseBaseUrl(Uri uri)
        {
            _httpHandlerOptions.RequestUri = uri;
            return this;
        }

        public HttpOptionsBuilder UseBaseUrl(string url)
        {
            return UseBaseUrl(UriHelper.BuildUri(url));
        }


        public HttpOptionsBuilder UseProxy(WebProxy proxy)
        {
            _httpHandlerOptions.IgnoreProxy = false;
            _httpHandlerOptions.Proxy = proxy;
            return this;
        }

        public HttpOptionsBuilder UseProxy(Uri proxy)
        {
            return UseProxy(new WebProxy(proxy));
        }

        public HttpOptionsBuilder UseProxy(Uri proxy, ICredentials credentials)
        {
            var webProxy = new WebProxy(proxy)
            {
                Credentials = credentials
            };
            return UseProxy(webProxy);
        }

        public HttpOptionsBuilder UseProxy(string proxy)
        {
            var uri = UriHelper.BuildUri(proxy);
            return UseProxy(uri);
        }

        public HttpOptionsBuilder UseProxy(string proxy, ICredentials credentials)
        {
            var uri = UriHelper.BuildUri(proxy);
            return UseProxy(uri, credentials);
        }

        public HttpOptionsBuilder IgnoreProxy(bool value)
        {
            _httpHandlerOptions.IgnoreProxy = true;
            return this;
        }


        public static implicit operator HttpHandlerOptions(HttpOptionsBuilder optionsOptionsBuilder)
        {
            return optionsOptionsBuilder._httpHandlerOptions;
        }
    }
}