using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using middler.Actions.ReverseProxy;
using middler.Common.Interfaces;
using middler.Common.SharedModels.Models;
using ProxyKit;

namespace middler.Actions.ForwardRequest
{
    public class ForwardRequestAction: MiddlerAction<ReverseProxyActionOptions>
    {
        internal static string DefaultActionType => "ForwardRequest";
        private const int StreamCopyBufferSize = 81920;
        public override string ActionType => DefaultActionType;
        internal static string HttpClientName { get; } = "Middler.Action.Proxy";

        public async Task ExecuteRequestAsync(HttpContext httpContext, IActionHelper actionHelper)
        {

            //var httpContext = actionContext.HttpContext;


            var baseUri = new Uri(actionHelper.BuildPathFromRoutData(Parameters.DestinationUrl));


            var uri = new Uri(UriHelper.BuildAbsolute(
                scheme: baseUri.Scheme, 
                host: HostString.FromUriComponent(baseUri),
                path: PathString.FromUriComponent(baseUri),
                query: httpContext.Request.QueryString));

            httpContext.Request.Path = PathString.FromUriComponent(uri);

            var up = new UpstreamHost(uri.ToString());
            
            var fc = httpContext.ForwardTo(up);
            if (Parameters.CopyXForwardedHeaders)
            {
                fc.CopyXForwardedHeaders();

            }

            if (Parameters.AddXForwardedHeaders)
            {
                fc.AddXForwardedHeaders();
            }
            
            var response = await fc.Send();
            await CopyProxyHttpResponse(httpContext, response);

        }

        private static async Task CopyProxyHttpResponse(HttpContext context, HttpResponseMessage responseMessage)
        {
            var response = context.Response;

            response.StatusCode = (int)responseMessage.StatusCode;
            foreach (var header in responseMessage.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            if (responseMessage.Content != null)
            {
                foreach (var header in responseMessage.Content.Headers)
                {
                    response.Headers[header.Key] = header.Value.ToArray();
                }
            }

            // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
            response.Headers.Remove("transfer-encoding");

            if (responseMessage.Content != null)
            {
                using (var responseStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    await responseStream.CopyToAsync(response.Body, StreamCopyBufferSize, context.RequestAborted).ConfigureAwait(false);
                    if (responseStream.CanWrite)
                    {
                        await responseStream.FlushAsync(context.RequestAborted).ConfigureAwait(false);    
                    }
                }
            }
        }
    }
}
