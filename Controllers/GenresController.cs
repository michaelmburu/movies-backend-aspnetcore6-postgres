using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Movies_API.Models.Genres;
using Movies_API.MovieContext;

namespace Movies_API.Controllers
{
   
    [Route("api/genres")]
    [ApiController] // No need to use ModelState.isValid, APIController does it automatically
    public class GenresController : ControllerBase
    {

        private readonly ILogger<GenresController> _logger;
        private MovieDBContext movieDBContext; 
        public GenresController(ILogger<GenresController> logger, MovieDBContext _movieDbContext)
        {
            _logger = logger;
            movieDBContext = _movieDbContext;
        }

        [HttpGet]
        //[ResponseCache(Duration = 60)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<Genre>>> Get()
        {
            _logger.LogInformation("Getting all information");
            return await movieDBContext.Genres.ToListAsync();
        }

        [HttpGet("{id:int}")]
        
        public ActionResult<Genre> Get(int id, [FromBody] string name)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Genre genre)
        {
            _logger.LogInformation($"Creating Genre {genre}");
            movieDBContext.Add(genre);
            await movieDBContext.SaveChangesAsync();
            return CreatedAtAction("Post", genre);
        }

        [HttpPut]
        public ActionResult Put([FromBody] Genre genre)
        {
            return NoContent();
        }

        [HttpDelete]
        public ActionResult<Genre> Delete([FromBody] Genre genre)
        {
            return NoContent();
        }

    }
}

