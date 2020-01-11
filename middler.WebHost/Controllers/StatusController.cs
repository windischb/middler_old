using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using middler.WebHost.ExtensionMethods;
using middler.WebHost.Models;

namespace middler.WebHost.Controllers
{
    [Route("api/status")]
    [Authorize]
    public class StatusController: Controller
    {

        private static DateTime ServiceStart { get; } = Process.GetCurrentProcess().StartTime;
        
        [HttpGet]
        public IActionResult GetStatus()
        {
            var status = new Status()
                {
                    ServiceName = this.GetType().Assembly.GetName().Name,
                    CurrentDateTime = DateTime.Now,
                    ClientIp = Request.FindSourceIp().FirstOrDefault()?.ToString(),
                    Version = this.GetType().Assembly.GetName().Version.ToString(),
                    UserAgent = Request.Headers["User-Agent"].ToString(),

                    ProxyServers = Request.FindSourceIp().Skip(1).Select(ip => ip.ToString()).ToArray(),
                    CurrentUser = this.User.Identity.Name ?? "Anonymous",
                    HostName = Environment.MachineName,
                    ServiceStart = ServiceStart,
                    ServiceRunningSince = DateTime.Now - ServiceStart
                };

                return Ok(status);
            
        }

    }
}
