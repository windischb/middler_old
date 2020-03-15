using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;

namespace middler.Action.Scripting.Commands.HttpCommand
{


    public class Http
    {

        public static HttpRequestBuilder Client(Action<HttpOptionsBuilder> options)
        {
            var builder = new HttpOptionsBuilder();
            options?.Invoke(builder);
            return new HttpRequestBuilder(builder);
        }

        public static HttpRequestBuilder Client(HttpHandlerOptions options)
        {
            return new HttpRequestBuilder(options);
        }

        public static HttpRequestBuilder Client()
        {
            return new HttpRequestBuilder();
        }

        public static HttpRequestBuilder Client(string url)
        {
            var builder = new HttpOptionsBuilder().UseBaseUrl(url);
            return new HttpRequestBuilder(builder);
        }

    }



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



        public Task<HttpResponseMessage> SendRequestMessageAsync(HttpRequestMessage httpRequestMessage)
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
            return cl.SendAsync(httpRequestMessage);
        }

        public Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod)
        {
            _httpRequestMessage.Method = httpMethod;
            return SendRequestMessageAsync(_httpRequestMessage);
        }

        public Task<HttpResponseMessage> GetAsync()
        {
            return SendAsync(HttpMethod.Get);
        }

        public Task<HttpResponseMessage> PostAsync()
        {
            return SendAsync(HttpMethod.Post);
        }

        public Task<HttpResponseMessage> PutAsync()
        {
            return SendAsync(HttpMethod.Put);
        }

        public Task<HttpResponseMessage> DeleteAsync()
        {
            return SendAsync(HttpMethod.Delete);
        }

    }

    public class HttpHandlerOptions
    {

        public Uri RequestUri { get; set; }
        public WebProxy Proxy { get; set; }
        public bool IgnoreProxy { get; set; }

    }

    internal class ReadonlyHttpHandlerOptions : IEquatable<ReadonlyHttpHandlerOptions>
    {
        public string DestinationHost { get; }
        public string ProxyHost { get; }
        public bool IgnoreProxy { get; }

        public ReadonlyHttpHandlerOptions(HttpHandlerOptions httpHandlerOptions)
        {
            DestinationHost = httpHandlerOptions.RequestUri?.Host;
            ProxyHost = httpHandlerOptions.Proxy?.Address?.Host;
            IgnoreProxy = httpHandlerOptions.IgnoreProxy;
        }


        public static bool operator ==(ReadonlyHttpHandlerOptions left, ReadonlyHttpHandlerOptions right) => Equals(left, right);

        public static bool operator !=(ReadonlyHttpHandlerOptions left, ReadonlyHttpHandlerOptions right) => !Equals(left, right);

        public bool Equals(ReadonlyHttpHandlerOptions other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(DestinationHost, other.DestinationHost) &&
                   Equals(ProxyHost, other.ProxyHost) &&
                   Equals(IgnoreProxy, other.IgnoreProxy);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ReadonlyHttpHandlerOptions)obj);
        }

        public override int GetHashCode()
        {
            return (DestinationHost, ProxyHost, IgnoreProxy).GetHashCode();
        }
    }

    public static class HttpHandlerFactory
    {
        private static readonly ConcurrentDictionary<ReadonlyHttpHandlerOptions, HttpMessageHandler> HttpHandlers = new ConcurrentDictionary<ReadonlyHttpHandlerOptions, HttpMessageHandler>();

        public static HttpMessageHandler Build(HttpHandlerOptions handlerOptions)
        {
            var roHttpHandlerOptions = new ReadonlyHttpHandlerOptions(handlerOptions);
            return HttpHandlers.GetOrAdd(roHttpHandlerOptions, _ => ValueFactory(handlerOptions));
        }

        private static HttpMessageHandler ValueFactory(HttpHandlerOptions handlerOptions)
        {
            var socketsHandler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromSeconds(60),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(20),
                MaxConnectionsPerServer = 2
            };

            if (handlerOptions.Proxy != null)
            {
                socketsHandler.Proxy = handlerOptions.Proxy;
            }

            if (handlerOptions.IgnoreProxy)
            {
                socketsHandler.UseProxy = false;
            }

            return socketsHandler;

        }
    }


    public interface IHttpRequestBuilder
    {
        Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod);
        Task<HttpResponseMessage> SendRequestMessageAsync(HttpRequestMessage httpMethod);

        Task<HttpResponseMessage> GetAsync();
        Task<HttpResponseMessage> PostAsync();
        Task<HttpResponseMessage> PutAsync();
        Task<HttpResponseMessage> DeleteAsync();

    }

    //public class HttpResponseMessage<T>: IDisposable
    //{

    //    private readonly HttpResponseMessage _httpResponseMessage;

    //    public Version Version => _httpResponseMessage.Version;
    //    public HttpContent Content => _httpResponseMessage.Content;
    //    public HttpStatusCode StatusCode => _httpResponseMessage.StatusCode;
    //    public string ReasonPhrase => _httpResponseMessage.ReasonPhrase;
    //    public HttpResponseHeaders Headers => _httpResponseMessage.Headers;
    //    public HttpResponseHeaders TrailingHeaders => _httpResponseMessage.TrailingHeaders;
    //    public HttpRequestMessage RequestMessage => _httpResponseMessage.RequestMessage;
    //    public bool IsSuccessStatusCode => _httpResponseMessage.IsSuccessStatusCode;
    //    public HttpResponseMessage EnsureSuccessStatusCode() => _httpResponseMessage.EnsureSuccessStatusCode();
    //    public override string ToString() => _httpResponseMessage.ToString();


    //    public T TypedContent { get; }

    //    internal HttpResponseMessage(HttpResponseMessage httpResponseMessage)
    //    {
    //        _httpResponseMessage = httpResponseMessage;


    //    }


    //    public void Dispose()
    //    {
    //        _httpResponseMessage?.Dispose();
    //    }
    //}

}
