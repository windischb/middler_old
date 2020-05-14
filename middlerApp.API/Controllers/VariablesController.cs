using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using middler.Variables;
using middler.Variables.LiteDB;
using middlerApp.API.Attributes;

namespace middlerApp.API.Controllers
{
    [ApiController]
    [Route("api/variables")]
    [AdminController]
    public class VariablesController: Controller
    {
        public VariableStore VariablesStore { get; }

        public VariablesController(VariableStore variablesStore)
        {
            VariablesStore = variablesStore;
        }


        [HttpGet("folders")]
        public IActionResult GetFolders()
        {
            return Ok(VariablesStore.GetFolderTree());
        }


    }
}
