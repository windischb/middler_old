using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using middler.Common.Interfaces;

namespace middler.Actions.ReverseProxy
{
    public static class ReverseProxyActionExtensions
    {
      

        public static IMiddlerOptionsBuilder AddReverseProxyAction(this IMiddlerOptionsBuilder optionsBuilder, string alias = null)
        {

            alias = !String.IsNullOrWhiteSpace(alias) ? alias : ReverseProxyAction.DefaultActionType;

            var httpClientBuilder = optionsBuilder.ServiceCollection
                .AddHttpClient(ReverseProxyAction.HttpClientName)
                .ConfigurePrimaryHttpMessageHandler(sp => new SocketsHttpHandler()
                {
                    AllowAutoRedirect = true,
                    UseCookies = false,
                    AutomaticDecompression = DecompressionMethods.All
                });

            optionsBuilder.ServiceCollection.AddTransient<ReverseProxyAction>();
            optionsBuilder.RegisterAction<ReverseProxyAction>(alias);

            return optionsBuilder;
        }
    }
}
