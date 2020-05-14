using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using middlerApp.API.Attributes;
using middlerApp.API.Helper;

namespace middlerApp.API.Controllers
{
    [ApiController]
    [Route("api/type-definition")]
    [AdminController]
    public class TypeDefinitionController : Controller
    {

        [HttpGet]
        public IActionResult GetDefinitions()
        {
            var typings =
                Directory.GetFileSystemEntries(PathHelper.GetFullPath("TypeDefinitions"))
                .Select(fe =>
                {
                    var f = new FileInfo(fe);
                    return new KeyValuePair<string, string>(f.Name, System.IO.File.ReadAllText(fe));
                }).ToList();

            return Ok(typings);

        }
    }
}
