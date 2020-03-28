using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using middler.DataStore;
using middlerApp.API.Attributes;

namespace middlerApp.API.Controllers
{
    [ApiController]
    [Route("api/variables")]
    [AdminController]
    public class VariablesController: Controller
    {
        public IDataStore DataStore { get; }

        public VariablesController(IDataStore dataStore)
        {
            DataStore = dataStore;
        }


        [HttpGet("folders")]
        public IActionResult GetFolders()
        {
            return Ok(DataStore.GetFolderTree());
        }

    }
}
