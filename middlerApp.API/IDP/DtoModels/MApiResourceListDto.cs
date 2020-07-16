

using System;
using System.Collections.Generic;

namespace middlerApp.API.IDP.DtoModels
{
    public class MApiResourceListDto
    {
        public Guid Id { get; set; }
        public bool Enabled { get; set; } = true;
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
      
    }
}
