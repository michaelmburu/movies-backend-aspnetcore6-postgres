using System;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Movies_API.DTO.Genre;
using Movies_API.DTO.PaginationDTO;
using Movies_API.Helpers;
using Movies_API.Models.Genres;
using Movies_API.MovieContext;

namespace Movies_API.Controllers
{
    [Route("api/genres")]
    [ApiController] // No need to use ModelState.isValid, APIController does it automatically
    public class GenresController : ControllerBase
    {

        private readonly ILogger<GenresController> _logger;
        private MovieDBContext _movieDBContext;
        private IMapper _mapper;

        public GenresController(ILogger<GenresController> logger, MovieDBContext movieDbContext, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _movieDBContext = movieDbContext;
        }

        [HttpGet] // api/genres
        public async Task<ActionResult<List<GenreDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            // Create a query object
            var queryable = _movieDBContext.Genres.AsQueryable();
            // Pass total amount of records in our db to client via header
            await HttpContext.InsertParametersPaginationFromHeader(queryable);

            //Apply pagination & order by genre
            var genres = await queryable.OrderBy(n => n.Name).Paginate(paginationDTO).ToListAsync();
            return _mapper.Map<List<GenreDTO>>(genres);
        }

        [HttpGet("{id:int}")]

        public async Task<ActionResult<GenreDTO>> Get(int id)
        {
            var genre = await _movieDBContext.Genres.FirstOrDefaultAsync(g => g.Id == id);
            if (genre == null)
            {
                return NotFound();
            }
            return _mapper.Map<GenreDTO>(genre);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GenreCreationDTO genreCreationDTO)
        {
            var genre = _mapper.Map<Genre>(genreCreationDTO);
            if (genre != null)
            {
                _movieDBContext.Add(genre);
                await _movieDBContext.SaveChangesAsync();
                _logger.LogInformation($"Added Genre {genre}");
                return CreatedAtAction("Post", genre);
            }
            else
            {
                return BadRequest(genreCreationDTO);
            }

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDTO genreCreationDTO)
        {
            var genre = await _movieDBContext.Genres.FirstOrDefaultAsync(g => g.Id == id);
            if (genre == null)
            {
                return NotFound();
            }
            genre = _mapper.Map(genreCreationDTO, genre);
            await _movieDBContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _movieDBContext.Genres.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }
            _movieDBContext.Remove(new Genre() { Id = id });
            await _movieDBContext.SaveChangesAsync();
            return Ok();
        }

    }
}

