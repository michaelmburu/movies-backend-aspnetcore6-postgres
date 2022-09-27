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
    [Route("api/movies")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
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

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<MovieDTO>> Get(int id)
        {
            var movie = await _movieDBContext.Movies
                                               .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                                               .Include(x => x.MovieTheatersMovies).ThenInclude(x => x.MovieTheater)
                                               .Include(x => x.MoviesActors).ThenInclude(x => x.Actor)
                                               .FirstOrDefaultAsync(x => x.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            var movieDTO = _mapper.Map<MovieDTO>(movie);
            movieDTO.Actors = movieDTO.Actors.OrderBy(x => x.Order).ToList();
            movieDTO.UserRating = _movieDBContext.Ratings.FirstOrDefault(x => x.MovieId == movieDTO.Id).Rate;
            movieDTO.AverageRating = movieDTO.UserRating;
            return movieDTO;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<LandingPageDTO>> Get()
        {
            var top = 6;
            var today = DateTime.Today.ToUniversalTime();

            var upComingReleases = await _movieDBContext.Movies
                .Where(x => x.ReleaseDate > today)
                .OrderBy(x => x.ReleaseDate)
                .Take(top)
                .ToListAsync();

            var inTheaters = await _movieDBContext.Movies
                .Where(x => x.InTheaters)
                .OrderBy(x => x.ReleaseDate)
                .Take(top)
                .ToListAsync();

            var landingPageDTO = new LandingPageDTO();
            landingPageDTO.UpComingReleases = _mapper.Map<List<MovieDTO>>(upComingReleases);
            landingPageDTO.InTheaters = _mapper.Map<List<MovieDTO>>(inTheaters);

            return landingPageDTO;

        }

        [HttpGet("filter")]
        [AllowAnonymous]
        public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] FilterMoviesDTO filterMoviesDTO)
        {
            var moviesQueryable = _movieDBContext.Movies.AsQueryable();

            if(!string.IsNullOrEmpty(filterMoviesDTO.Title))
            {
                moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(filterMoviesDTO.Title));
            }

            if (filterMoviesDTO.InTheaters)
            {
                moviesQueryable = moviesQueryable.Where(x => x.InTheaters);
            }

            if(filterMoviesDTO.UpComingReleases)
            {
                var today = DateTime.Today.ToUniversalTime();
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }

            if(filterMoviesDTO.GenreId != 0)
            {
                moviesQueryable = moviesQueryable.Where(x => x.MoviesGenres.Select(y => y.GenreId).Contains(filterMoviesDTO.GenreId));
            }

            await HttpContext.InsertParametersPaginationFromHeader(moviesQueryable);

            var movies = await moviesQueryable.OrderBy(x => x.Title).Paginate(filterMoviesDTO.PaginationDTO).ToListAsync();

            return _mapper.Map<List<MovieDTO>>(movies);
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

        [HttpGet("putget/{id:int}")]
        public async Task<ActionResult<MoviePutGetDTO>> PutGet(int id)
        {

            var movieActionResult = await Get(id);
            if(movieActionResult.Result is NotFoundResult) { return NotFound(); }

            var movie = movieActionResult?.Value;

            if(movie == null)
            {
                return NotFound();
            }

            var genresSelectedIds = movie.Genres.Select(x => x.Id).ToList();
            var nonSelectedGenres = await _movieDBContext.Genres.Where(x => !genresSelectedIds.Contains(x.Id))
                                                            .ToListAsync();

            var movieTheatersIds = movie.MovieTheaters.Select(x => x.Id).ToList();
            var nonSelectedMovieTheaters = await _movieDBContext.MovieTheaters.Where(x => !movieTheatersIds.Contains(x.Id))
                                                            .ToListAsync();

            var noneSelectedGenresDTO = _mapper.Map<List<GenreDTO>>(nonSelectedGenres);
            var noneSelectedMovieTheatersDTO = _mapper.Map<List<MovieTheaterDTO>>(nonSelectedMovieTheaters);

            var response = new MoviePutGetDTO {
                Movie = movie,
                SelectedGenres = movie.Genres,
                NoneSelectedGenres = noneSelectedGenresDTO,
                SelectedMovieTheaters = movie.MovieTheaters,
                NoneSelectedMovieTheaters = noneSelectedMovieTheatersDTO,
                Actors = movie.Actors
            };

            return response;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = await _movieDBContext.Movies.Include(x => x.MoviesActors)
                                                .Include(x => x.MoviesGenres)
                                                .Include(x => x.MovieTheatersMovies)
                                                .FirstOrDefaultAsync(x => x.Id == id);
            if(movie == null) { return NotFound(); }
            movie = _mapper.Map(movieCreationDTO, movie);

            if(movieCreationDTO.Poster != null)
            {
                //Add new image here
            }

            AnnotateActorsOrder(movie);
            await _movieDBContext.SaveChangesAsync();
            return NoContent();

        }

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromForm] MovieCreationDTO movieCreationDTO)
        {
            movieCreationDTO.ReleaseDate = movieCreationDTO.ReleaseDate.ToUniversalTime();
            var movie = _mapper.Map<Movie>(movieCreationDTO);
            movie.Poster = "https://en.wikipedia.org/wiki/File:Black_Panther_Wakanda_Forever_poster.jpg";
            //if(movieCreationDTO.Poster != null)
            //{
            //    movie.Poster = await _fileStorageService.SaveFile(container, movieCreationDTO.Poster);
            //}

            AnnotateActorsOrder(movie);
            _movieDBContext.Add(movie);
            await _movieDBContext.SaveChangesAsync();

            return movie.Id;
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

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var movie = await _movieDBContext.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if(movie == null) { return NotFound(); }

            _movieDBContext.Remove(movie);
            await _movieDBContext.SaveChangesAsync();
            // await _fileStorageService.DeleteFile(movie.poster, container);
            return NoContent();
        }
    }
}

