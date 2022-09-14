using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies_API.DTO.MovieTheater;
using Movies_API.Models.Genres;
using Movies_API.Models.MovieTheater;
using Movies_API.MovieContext;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Movies_API.Controllers
{
    [ApiController]
    [Route("api/movietheaters")]
    public class MovieTheatersController : Controller
    {
        private readonly MovieDBContext _movieDBContext;
        private readonly IMapper _mapper;
        public MovieTheatersController(MovieDBContext movieDBContext, IMapper mapper)
        {
            _movieDBContext = movieDBContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<MovieTheaterDTO>>> Get()
        {
            var entities = await _movieDBContext.MovieTheaters.ToListAsync();
            return  _mapper.Map<List<MovieTheaterDTO>>(entities);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MovieTheaterDTO>> Get(int id)
        {
            var movieTheater = await _movieDBContext.MovieTheaters.FirstOrDefaultAsync(x => x.Id == id);
            if(movieTheater == null)
            {
                return NotFound();
            }

            return _mapper.Map<MovieTheaterDTO>(movieTheater);

        }

        [HttpPost]
        public async Task<ActionResult> Post(MovieTheaterCreationDTO movieCreationDTO)
        {
            var movieTheater = _mapper.Map<MovieTheater>(movieCreationDTO);
            _movieDBContext.Add(movieTheater);
            await _movieDBContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, MovieTheaterCreationDTO movieCreationDTO)
        {
            var movieTheater = await _movieDBContext.MovieTheaters.FirstOrDefaultAsync(x => x.Id == id);

            if(movieTheater == null)
            {
                return NotFound();
            }

            movieTheater = _mapper.Map(movieCreationDTO, movieTheater);
            await _movieDBContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _movieDBContext.MovieTheaters.FirstOrDefaultAsync(x => x.Id == id);
            if (exists == null)
            {
                return NotFound();
            }
            _movieDBContext.Remove(new Genre() { Id = id });
            await _movieDBContext.SaveChangesAsync();
            return Ok();
        }
    }
}

