using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using middlerApp.API.Attributes;
using middlerApp.Data;

namespace middlerApp.API.Controllers
{
    [ApiController]
    [Route("api/variables")]
    [AdminController]
    public class VariablesController: Controller
    {
        public VariablesRepository VariablesStore { get; }

        public VariablesController(VariablesRepository variablesStore)
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
