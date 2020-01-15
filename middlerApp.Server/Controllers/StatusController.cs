using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using middlerApp.Server.ExtensionMethods;
using middlerApp.Server.Models;

namespace middlerApp.Server.Controllers
{
    [Route("api/status")]

    public class StatusController: Controller
    {

        private static DateTime ServiceStart { get; } = Process.GetCurrentProcess().StartTime;
        private IWebHostEnvironment hostEnvironment;


        public StatusController(IWebHostEnvironment environment)
        {
            hostEnvironment = environment;
        }

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
                    ServiceRunningSince = DateTime.Now - ServiceStart,
                    ContentRoot = hostEnvironment.ContentRootPath,
                    WebRoot = hostEnvironment.WebRootPath
                };

                return Ok(status);
            
        }

    }
}
