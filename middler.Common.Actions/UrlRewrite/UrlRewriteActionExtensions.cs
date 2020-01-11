using Microsoft.Extensions.DependencyInjection;
using middler.Common.Interfaces;

namespace middler.Common.Actions.UrlRewrite
{
    public static class UrlRewriteActionExtensions
    {
        public static IMiddlerOptionsBuilder AddUrlRewriteAction(this IMiddlerOptionsBuilder optionsBuilder, string alias = "UrlRewrite")
        {

            optionsBuilder.ServiceCollection.AddTransient<UrlRewriteAction>();
            optionsBuilder.RegisterAction<UrlRewriteAction>(alias);

            return optionsBuilder;
        }
        
    }
}