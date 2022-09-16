using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies_API.DTO;
using Movies_API.Helpers;
using Movies_API.Models;
using Movies_API.MovieContext;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Movies_API.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieDBContext _movieDBContext;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private string container = "movies";

        public MoviesController(MovieDBContext moviesDBContext, IMapper mapper, IFileStorageService fileStorageService)
        {
            _mapper = mapper;
            _movieDBContext = moviesDBContext;
            _fileStorageService = fileStorageService;
        }


        [HttpGet("PostGet")]
        public async Task<ActionResult<MoviePostGetDTO>> PostGet()
        {
            var movieTheaters = await _movieDBContext.MovieTheaters.ToListAsync();
            var genres = await _movieDBContext.Genres.ToListAsync();

            var movieTheaterDTO = _mapper.Map<List<MovieTheaterDTO>>(movieTheaters);
            var genresDTO = _mapper.Map<List<GenreDTO>>(genres);

            return new MoviePostGetDTO() { Genres = genresDTO, MovieTheaters = movieTheaterDTO };
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = _mapper.Map<Movie>(movieCreationDTO);
            if(movieCreationDTO.Poster == null)
            {
                movie.Poster = await _fileStorageService.SaveFile(container, movieCreationDTO.Poster);
            }

            AnnotateActorsOrder(movie);
            _movieDBContext.Add(movie);
            await _movieDBContext.SaveChangesAsync();

            return Created("actors", movieCreationDTO);
        }

        //Store in database the correct order
        private void AnnotateActorsOrder(Movie movie)
        {
            if(movie.MoviesActors != null)
            {
                for(int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }
    }
}

