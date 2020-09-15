using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using middlerApp.API.Attributes;
using SignalARRR.Server;
using SignalARRR.Server.ExtensionMethods;

namespace middlerApp.API.Controllers
{
    [Route("api/test")]
    [AdminController]
    public class TestController: Controller
    {
        private ClientManager ClientManager { get; }

        public TestController(ClientManager clientManager)
        {
            ClientManager = clientManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {

            var result = await ClientManager.GetAllClients().InvokeAllAsync<string>("Test", new object[] {"abc", 123}, CancellationToken.None);

            return Ok(result);
        }
    }
}
