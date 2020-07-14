using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using middlerApp.API.IDP.Models;

namespace middlerApp.API.IDP.DtoModels
{
    public class MRoleDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public ICollection<MUserDto> Users { get; set; } = new List<MUserDto>();
    }
}
