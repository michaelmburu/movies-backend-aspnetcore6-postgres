using System;
namespace Movies_API.DTO
{
    public class MoviePutGetDTO
    {
        public MovieDTO Movie { get; set; }
        public List<GenreDTO> SelectedGenres { get; set; }
        public List<GenreDTO> NoneSelectedGenres { get; set; }
        public List<MovieTheaterDTO> SelectedMovieTheaters { get; set; }
        public List<MovieTheaterDTO> NoneSelectedMovieTheaters { get; set; }
        public List<ActorsMovieDTO> Actors { get; set; }
    }
}

