using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies_API.DTO;
using Movies_API.Helpers;
using Movies_API.Models;
using Movies_API.MovieContext;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Movies_API.Controllers
{  
    [Route("api/movietheaters")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
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
        public async Task<ActionResult<List<MovieTheaterDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = _movieDBContext.MovieTheaters.AsQueryable();
            await HttpContext.InsertParametersPaginationFromHeader(queryable);
            var entities = await queryable.OrderBy(x => x.Name).ToListAsync();
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

