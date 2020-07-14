using System;
using System.ComponentModel.DataAnnotations;

namespace middlerApp.API.IDP.Models
{
    public class MUserLogin : IConcurrencyAware
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(200)]
        [Required]
        public string Provider { get; set; }

        [MaxLength(200)]
        [Required]
        public string ProviderIdentityKey { get; set; }


        [Required]
        public Guid UserId { get; set; }

        public MUser User { get; set; }
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }
}
