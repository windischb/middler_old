
using System;
using System.Collections.Generic;
using middlerApp.API.IDP.Storage.Entities;

namespace middlerApp.API.IDP.DtoModels
{
    public class MScopeListDto
    {
        public Guid Id { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
       
    }
}
