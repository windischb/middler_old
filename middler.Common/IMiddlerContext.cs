using System;
using System.Collections.Generic;
using System.Text;

namespace middler.Common
{
    public interface IMiddlerContext
    {
        IMiddlerRequestContext Request { get; }

        IMiddlerResponseContext Response { get; }

        IServiceProvider RequestServices { get; }
        void ForwardBody();
    }
}
