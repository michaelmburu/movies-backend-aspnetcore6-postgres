using System;
using System.ComponentModel.DataAnnotations;

namespace Movies_API.DTO
{
    public class RatingDTO
    {
        [Range(1, 5)]
        public int Rating { get; set; }
        public int MovieId { get; set; }
    }
}

