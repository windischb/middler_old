using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace middlerApp.API.IDP.Models
{
    public class MRole
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public ICollection<MUserRoles> UserRoles { get; set; } = new List<MUserRoles>();
    }
}
