using System;

namespace Movies_API.Models
{
    public class MovieTheatersMovies
    {
        public int MovieTheaterId { get; set; }
        public int MovieId { get; set; }
        public MovieTheater MovieTheater { get; set; }
        public Movie Movie { get; set; }
    }
}


