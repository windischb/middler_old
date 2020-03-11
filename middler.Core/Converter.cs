using System;
using System.Collections.Generic;
using System.Text;

namespace middler.Core
{
    public class Converter
    {
        private static readonly Lazy<Reflectensions.Json> lazyJson = new Lazy<Reflectensions.Json>(() => new Reflectensions.Json()

        );

        public static Reflectensions.Json Json => lazyJson.Value;

    }
}
