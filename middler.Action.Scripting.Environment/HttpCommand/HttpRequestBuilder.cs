using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using Nito.AsyncEx.Synchronous;
using Reflectensions.Helper;

namespace middler.Scripting.HttpCommand
{
    public class HttpRequestBuilder : IHttpRequestBuilder
    {
        private readonly HttpRequestMessage _httpRequestMessage = new HttpRequestMessage();

        private readonly HttpHandlerOptions _httpHandlerOptions;

        private readonly ConcurrentDictionary<string, StringValues> _queryParameters = new ConcurrentDictionary<string, StringValues>();


        public HttpRequestBuilder() { }

        public HttpRequestBuilder(HttpHandlerOptions httpHandlerOptions)
        {
            _httpHandlerOptions = httpHandlerOptions;
        }


        public HttpRequestBuilder UseUrl(Uri uri)
        {
            _httpRequestMessage.RequestUri = uri;
            return this;
        }

        public HttpRequestBuilder UseUrl(string url)
        {
            if (_httpHandlerOptions?.RequestUri != null)
            {
                _httpRequestMessage.RequestUri = new Uri(_httpHandlerOptions.RequestUri, url);
            }
            else
            {
                _httpRequestMessage.RequestUri = UriHelper.BuildUri(url);
            }
            return this;
        }


        public HttpRequestBuilder AddHeader(string key, params string[] value)
        {
            if (String.IsNullOrWhiteSpace(key))
                return this;

            key = key.ToLower();

            _httpRequestMessage.Headers.Add(key, value);

            return this;

        }

        public HttpRequestBuilder SetHeader(string key, params string[] value)
        {
            if (String.IsNullOrWhiteSpace(key))
                return this;

            key = key.ToLower();
            _httpRequestMessage.Headers.Remove(key);
            _httpRequestMessage.Headers.Add(key, value);

            return this;
        }

        public HttpRequestBuilder SetContentType(string contentType)
        {
            _httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return this;
        }

        public HttpRequestBuilder AddQueryParam(string key, params string[] value)
        {
            if (String.IsNullOrWhiteSpace(key))
                return this;

            key = key.ToLower();

            _queryParameters.AddOrUpdate(key, s => {
                return new StringValues(value);
            }, (s, sv) => {
                var temp = sv.ToList();
                temp.AddRange(value);
                return new StringValues(temp.ToArray());

            });

            return this;
        }

        public HttpRequestBuilder SetQueryParam(string key, params string[] value)
        {
            if (String.IsNullOrWhiteSpace(key))
                return this;

            key = key.ToLower();

            _queryParameters.TryRemove(key, out var _);
            _queryParameters.TryAdd(key, new StringValues(value));
            
            return this;
        }



        public async Task<HttpResponse> SendRequestMessageAsync(HttpRequestMessage httpRequestMessage)
        {
            if (httpRequestMessage.RequestUri == null)
            {
                httpRequestMessage.RequestUri = _httpHandlerOptions?.RequestUri;
            }

            if (_queryParameters.Any())
            {
                
                var qb = new QueryBuilder();
                if (!String.IsNullOrWhiteSpace(httpRequestMessage.RequestUri?.Query))
                {
                    var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(httpRequestMessage.RequestUri.Query);
                    foreach (var (key, value) in query)
                    {
                        qb.Add(key, value.ToArray());
                    }
                }
                foreach (var (key, value) in _queryParameters)
                {
                    qb.Add(key, value.ToArray());
                }

                var uriBuilder = new UriBuilder(httpRequestMessage.RequestUri);
                uriBuilder.Query = qb.ToQueryString().ToString();

                httpRequestMessage.RequestUri = uriBuilder.Uri;

            }
            var cl = new HttpClient(HttpHandlerFactory.Build(_httpHandlerOptions ?? new HttpHandlerOptions()));
            var respMsg = await cl.SendAsync(httpRequestMessage);
            return new HttpResponse(respMsg);
        }

        public Task<HttpResponse> SendAsync(HttpMethod httpMethod)
        {
            _httpRequestMessage.Method = httpMethod;
            return SendRequestMessageAsync(_httpRequestMessage);
        }

        public Task<HttpResponse> GetAsync()
        {
            return SendAsync(HttpMethod.Get);
        }

        public Task<HttpResponse> PostAsync()
        {
            return SendAsync(HttpMethod.Post);
        }

        public Task<HttpResponse> PutAsync()
        {
            return SendAsync(HttpMethod.Put);
        }

        public Task<HttpResponse> DeleteAsync()
        {
            return SendAsync(HttpMethod.Delete);
        }



        public HttpResponse SendRequestMessage(HttpRequestMessage httpRequestMessage)
        {
            return SendRequestMessageAsync(httpRequestMessage).WaitAndUnwrapException();
        }

        public HttpResponse Send(HttpMethod httpMethod)
        {
            return SendAsync(httpMethod).WaitAndUnwrapException();
        }


        public HttpResponse Get()
        {
            return GetAsync().WaitAndUnwrapException();
        }
    }
}