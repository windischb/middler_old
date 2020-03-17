using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using middler.Scripting.HttpCommand.Converters;
using Newtonsoft.Json;
using Nito.AsyncEx.Synchronous;
using Reflectensions.Helper;

namespace middler.Scripting.HttpCommand
{
    public class HttpRequestBuilder : IHttpRequestBuilder
    {
        private readonly HttpRequestMessage _httpRequestMessage = new HttpRequestMessage();

        private readonly HttpHandlerOptions _httpHandlerOptions;

        private readonly ConcurrentDictionary<string, StringValues> _queryParameters = new ConcurrentDictionary<string, StringValues>();

        private string ContentType { get; set; }

        public HttpRequestBuilder() { }

        public HttpRequestBuilder(HttpHandlerOptions httpHandlerOptions)
        {
            _httpHandlerOptions = httpHandlerOptions;
        }


        public HttpRequestBuilder UsePath(Uri uri)
        {
            _httpRequestMessage.RequestUri = uri;
            return this;
        }

        public HttpRequestBuilder UsePath(string url)
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
            ContentType = contentType;
            //_httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
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

        public Task<HttpResponse> SendAsync(string httpMethod, object content = null)
        {
            return SendAsync(new HttpMethod(httpMethod), content);
        }

        public async Task<HttpResponse> SendAsync(HttpMethod httpMethod, object content = null)
        {
            _httpRequestMessage.Method = httpMethod;
            if (content != null)
            {
                _httpRequestMessage.Content = await CreateHttpContent(content);
            }

            if (!String.IsNullOrWhiteSpace(ContentType))
            {
                _httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
            }
            return await SendRequestMessageAsync(_httpRequestMessage);
        }

        public Task<HttpResponse> GetAsync()
        {
            return SendAsync(HttpMethod.Get);
        }

        public Task<HttpResponse> PostAsync(object content)
        {
            return SendAsync(HttpMethod.Post, content);
        }

        public Task<HttpResponse> PutAsync(object content)
        {
            return SendAsync(HttpMethod.Put, content);
        }

        public Task<HttpResponse> PatchAsync(object content)
        {
            return SendAsync(HttpMethod.Patch, content);
        }

        
        public Task<HttpResponse> DeleteAsync(object content)
        {
            return SendAsync(HttpMethod.Delete, content);
        }

        public Task<HttpResponse> DeleteAsync()
        {
            return DeleteAsync(null);
        }

        public HttpResponse Send(string httpMethod, object content = null)
        {
            return SendAsync(httpMethod, content).WaitAndUnwrapException();
        }

        public HttpResponse Send(HttpMethod httpMethod, object content = null)
        {
            return SendAsync(httpMethod, content).WaitAndUnwrapException();
        }

        public HttpResponse SendRequestMessage(HttpRequestMessage httpRequestMessage)
        {
            return SendRequestMessageAsync(httpRequestMessage).WaitAndUnwrapException();
        }

        public HttpResponse Get()
        {
            return GetAsync().WaitAndUnwrapException();
        }

        public HttpResponse Post(object content)
        {
            return PostAsync(content).WaitAndUnwrapException();
        }

        public HttpResponse Put(object content)
        {
            return PutAsync(content).WaitAndUnwrapException();
        }

        public HttpResponse Patch(object content)
        {
            return PatchAsync(content).WaitAndUnwrapException();
        }

        public HttpResponse Delete(object content)
        {
            return DeleteAsync(content).WaitAndUnwrapException();
        }
        public HttpResponse Delete()
        {
            return Delete(null);
        }


        private async Task<HttpContent> CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content == null)
                return null;

            var ms = new MemoryStream();

            if (content is string str)
            {
                httpContent = new StringContent(str, Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                return httpContent;
            }

            await CreateJsonHttpContent(content, ms);
            ms.Seek(0, SeekOrigin.Begin);
            httpContent = new StreamContent(ms);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpContent;
        }

        private async Task CreateJsonHttpContent(object content, Stream stream)
        {
            await new JsonContentConverter().ConvertToStream(content, stream);
        }
       
    }
}