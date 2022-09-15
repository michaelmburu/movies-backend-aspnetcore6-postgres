using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = _mapper.Map<Movie>(movieCreationDTO);
            if(string.IsNullOrEmpty(movie.Poster))
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

