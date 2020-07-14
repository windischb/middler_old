using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace middlerApp.API.IDP.Models
{
    public class MUserRoles
    {
        public Guid UserId { get; set; }
        public MUser User { get; set; }

        public Guid RoleId { get; set; }
        public MRole Role { get; set; }
       
       
    }
}
