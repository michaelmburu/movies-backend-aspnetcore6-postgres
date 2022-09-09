using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Movies_API.Models.Genres;
using Movies_API.MovieContext;

namespace Movies_API.Controllers
{
   
    [Route("api/genres")]
    [ApiController]
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
            return await movieDBContext.Ge
        }

        [HttpGet("{id:int}")]
        
        public ActionResult<Genre> Get(int id, [FromBody] string name)
        {
            return Ok();
        }

        [HttpPost]
        public ActionResult<Genre> Post([FromBody] Genre genre)
        {
            return NoContent();
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

