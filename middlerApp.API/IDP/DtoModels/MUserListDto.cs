using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using middlerApp.API.IDP.Models;
using Org.BouncyCastle.Asn1;

namespace middlerApp.API.IDP.DtoModels
{
    public class MUserListDto
    {

        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public bool EmailConfirmed { get; set; }
        
        public bool Active { get; set; }

    }
}
