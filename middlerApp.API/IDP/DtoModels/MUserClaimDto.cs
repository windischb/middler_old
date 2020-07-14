using System;
using System.ComponentModel.DataAnnotations;
using middlerApp.API.IDP.Models;

namespace middlerApp.API.IDP.DtoModels
{
    public class MUserClaimDto
    {
        public Guid? Id { get; set; }

        [MaxLength(250)]
        [Required]
        public string Type { get; set; }

        [MaxLength(250)]
        [Required]
        public string Value { get; set; }

        public string ConcurrencyStamp { get; set; }

    }
}
