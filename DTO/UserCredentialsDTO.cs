using System;
using System.ComponentModel.DataAnnotations;

namespace Movies_API.DTO
{
    public class UserCredentialsDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

