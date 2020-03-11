
namespace middler.Actions.ReverseProxy
{
    public class ReverseProxyActionOptions
    {
        public string DestinationUrl { get; set; }
        public bool UserIntermediateStream { get; set; }
        public bool AddXForwardedHeaders { get; set; }
        public bool CopyXForwardedHeaders { get; set; }
    }

    public class ReverseProxyActionOptionsBuilder
    {
        private ReverseProxyActionOptions Options { get; }

        public ReverseProxyActionOptionsBuilder(string destinationUrl)
        {
            Options = new ReverseProxyActionOptions {
                DestinationUrl = destinationUrl
            };
        }

        public ReverseProxyActionOptionsBuilder AddXForwardedHeaders(bool value)
        {
            Options.AddXForwardedHeaders = value;
            return this;
        }

        public ReverseProxyActionOptionsBuilder AddXForwardedHeaders()
        {
            return AddXForwardedHeaders(true);
        }

        public ReverseProxyActionOptionsBuilder CopyXForwardedHeaders(bool value)
        {
            Options.AddXForwardedHeaders = value;
            return this;
        }

        public ReverseProxyActionOptionsBuilder CopyXForwardedHeaders()
        {
            return CopyXForwardedHeaders(true);
        }

        public ReverseProxyActionOptionsBuilder UserIntermediateStream(bool value)
        {
            Options.UserIntermediateStream = value;
            return this;
        }

        public ReverseProxyActionOptionsBuilder UserIntermediateStream()
        {
            return UserIntermediateStream(true);
        }


        public static implicit operator ReverseProxyActionOptions(ReverseProxyActionOptionsBuilder builder)
        {
            return builder.Options;
        }
    }
}
