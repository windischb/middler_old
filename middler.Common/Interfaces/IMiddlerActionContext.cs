using Microsoft.AspNetCore.Http;

namespace middler.Common.Interfaces {

    public interface IMiddlerActionContext {

        IMiddlerActionRequest Request { get; }
        IMiddlerActionHelper Helper { get; }
        HttpContext HttpContext { get; }
        AutoStream ResponseBody { get; }

    }
}
