using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Movies_API.DTO;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Movies_API.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("create")]
        public async Task<ActionResult<AuthenticationResponseDTO>> Create([FromBody] UserCredentialsDTO userCredentialsDTO)
        {
            var user = new IdentityUser { UserName = userCredentialsDTO.Email, Email = userCredentialsDTO.Email };
            var result = await _userManager.CreateAsync(user, userCredentialsDTO.Password);

            if(result.Succeeded)
            {
                return BuildToken(userCredentialsDTO);            
            }
            else
            {
                return BadRequest(result.Errors.Select(x => x.Description));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponseDTO>> Login([FromBody] UserCredentialsDTO userCredentialsDTO)
        {
            var result = await _signInManager.PasswordSignInAsync(userCredentialsDTO.Email, userCredentialsDTO.Password, isPersistent: false, lockoutOnFailure: false);
            if(result.Succeeded)
            {
                return BuildToken(userCredentialsDTO);
            }
            else
            {
                return BadRequest("Incorrect Login");
            }
        }

        private AuthenticationResponseDTO BuildToken(UserCredentialsDTO userCredentialsDTO)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentialsDTO.Email)
            };

            var jwtSigningKey = _configuration["ConnectionStrings:keyjwt"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey));

            var signInCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenExpirationTime = DateTime.UtcNow.AddYears(1);

            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: tokenExpirationTime, signingCredentials: signInCredentials);

            return new AuthenticationResponseDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = tokenExpirationTime  
            };
        }

    }
}

