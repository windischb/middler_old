using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;

namespace middler.Scripting.HttpCommand.Converters
{
    public abstract class ContentConverter
    {
        public abstract bool CanConvert(HttpContentHeaders contentHeaders);

        public abstract T ConvertToObject<T>(Stream stream);
    }
}
