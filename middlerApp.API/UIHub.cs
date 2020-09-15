using System;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using SignalARRR.Server;

namespace middlerApp.API {

    [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
    public class UIHub : HARRR {
        public UIHub(IServiceProvider serviceProvider) : base(serviceProvider) {
        }
    }
}
