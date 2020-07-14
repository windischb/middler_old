using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace middler.IDP.Models
{
    public class MUser : IConcurrencyAware
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(200)]
        [Required]
        public string Subject { get; set; }

        [MaxLength(200)]
        public string Username { get; set; }

        [MaxLength(200)]
        public string Password { get; set; }

        [Required]
        public bool Active { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(200)]
        public string SecurityCode { get; set; }

        public DateTime SecurityCodeExpirationDate { get; set; }

        [ConcurrencyCheck]
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        public ICollection<MUserClaim> Claims { get; set; } = new List<MUserClaim>();
        public ICollection<MUserLogin> Logins { get; set; } = new List<MUserLogin>();
        public ICollection<MUserSecret> Secrets { get; set; } = new List<MUserSecret>();

    }
}
