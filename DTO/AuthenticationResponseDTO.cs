using System;
namespace Movies_API.DTO
{
    public class AuthenticationResponseDTO
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}

