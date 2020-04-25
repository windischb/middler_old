using System;
using Newtonsoft.Json.Converters;
using Reflectensions.JsonConverters;

namespace middlerApp.API.Helper
{
    public static class Converter
    {

        private static readonly Lazy<Reflectensions.Json> lazyJson = new Lazy<Reflectensions.Json>(() => new Reflectensions.Json()
            .RegisterJsonConverter<StringEnumConverter>()
            .RegisterJsonConverter<DefaultDictionaryConverter>()
        );

        public static Reflectensions.Json Json => lazyJson.Value;


        public static T CopyTo<T>(object @object)
        {
            var json = Json.ToJson(@object);
            return Json.ToObject<T>(json);
        }

    }

}
