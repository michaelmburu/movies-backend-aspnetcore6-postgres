using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movies_API.DTO;
using Movies_API.Helpers;
using Movies_API.MovieContext;

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
        private readonly MovieDBContext _movieDBContext;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, MovieDBContext movieDBContext, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _movieDBContext = movieDBContext;
            _mapper = mapper;
        }

        [HttpGet("listUsers")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<List<UserDTO>>> GetListUsers([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = _movieDBContext.Users.AsQueryable();
            await HttpContext.InsertParametersPaginationFromHeader(queryable);
            var users = await queryable.OrderBy(x => x.Email).Paginate(paginationDTO).ToListAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }

        [HttpPost("makeAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult> MakeAdmin([FromBody] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.AddClaimAsync(user, new Claim("role", "admin"));
            return NoContent();
        }

        [HttpPost("removeAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult> RemoveAdmin([FromBody] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.RemoveClaimAsync(user, new Claim("role", "admin"));
            return NoContent();
        }


        [HttpPost("create")]
        public async Task<ActionResult<AuthenticationResponseDTO>> Create([FromBody] UserCredentialsDTO userCredentialsDTO)
        {
            var user = new IdentityUser { UserName = userCredentialsDTO.Email, Email = userCredentialsDTO.Email };
            var result = await _userManager.CreateAsync(user, userCredentialsDTO.Password);

            if(result.Succeeded)
            {
                return await BuildToken(userCredentialsDTO);            
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
                return await BuildToken(userCredentialsDTO);
            }
            else
            {
                return BadRequest("Incorrect Login");
            }
        }

        private async Task<AuthenticationResponseDTO> BuildToken(UserCredentialsDTO userCredentialsDTO)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentialsDTO.Email)
            };


            var user = await _userManager.FindByNameAsync(userCredentialsDTO.Email);
            var userClaims = await _userManager.GetClaimsAsync(user);

            claims.AddRange(userClaims);

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

