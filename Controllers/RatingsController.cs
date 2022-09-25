using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies_API.DTO;
using Movies_API.Models;
using Movies_API.MovieContext;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Movies_API.Controllers
{
    [Route("api/ratings")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly MovieDBContext _movieContext;
        private readonly UserManager<IdentityUser> _userManager;

        public RatingsController(MovieDBContext movieContext, UserManager<IdentityUser> userManager)
        {
            _movieContext = movieContext;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post([FromBody] RatingDTO ratingDTO)
        {
            var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
            var user = await _userManager.FindByEmailAsync(email);
            var userId = user.Id;

            var currentUserMovieRating = await _movieContext.Ratings.FirstOrDefaultAsync(x => x.MovieId == ratingDTO.MovieId && x.UserId == userId);

            if(currentUserMovieRating == null)
            {
                var rating = new Rating()
                {
                    MovieId = ratingDTO.MovieId,
                    Rate = ratingDTO.Rating,
                    UserId = userId
                };

                _movieContext.Add(rating);
            }
            else
            {
                currentUserMovieRating.Rate = ratingDTO.Rating;
            }

            await _movieContext.SaveChangesAsync();
            return NoContent();
        }
    }
}

