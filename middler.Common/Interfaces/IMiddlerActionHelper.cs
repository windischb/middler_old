using System;
using System.Collections.Generic;
using System.Text;

namespace middler.Common.Interfaces
{
    public interface IMiddlerActionHelper
    {
        AutoStream CreateDefaultAutoStream();
        string BuildPathFromRoutData(string template);

    }
}
