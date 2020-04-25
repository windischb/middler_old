using System;

namespace middler.Variables
{
    public class StoreConfig : IFSConfig
    {
        public string RootPath { get; internal set; }
    }

    public class StoreConfigBuilder
    {
        private StoreConfig _config = new StoreConfig();

        public StoreConfigBuilder UseRootPath(string path)
        {
            _config.RootPath = path;
            return this;
        }


        public static implicit operator StoreConfig(StoreConfigBuilder builder)
        {
            return builder._config;
        }

        public static implicit operator StoreConfigBuilder(Action<StoreConfigBuilder> builder)
        {
            var optsBuilder = new StoreConfigBuilder();
            builder?.Invoke(optsBuilder);
            return optsBuilder;
        }
    }
}
