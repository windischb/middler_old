using System;
using System.Collections.Generic;
using System.Text;

namespace middler.DataStore
{
    public class DataStoreConfig : IFSConfig
    {
        public string RootPath { get; internal set; }
    }

    public class DataStoreConfigBuilder
    {
        private DataStoreConfig _config = new DataStoreConfig();

        public DataStoreConfigBuilder UseRootPath(string path)
        {
            _config.RootPath = path;
            return this;
        }


        public static implicit operator DataStoreConfig(DataStoreConfigBuilder builder)
        {
            return builder._config;
        }

        public static implicit operator DataStoreConfigBuilder(Action<DataStoreConfigBuilder> builder)
        {
            var optsBuilder = new DataStoreConfigBuilder();
            builder?.Invoke(optsBuilder);
            return optsBuilder;
        }
    }
}
