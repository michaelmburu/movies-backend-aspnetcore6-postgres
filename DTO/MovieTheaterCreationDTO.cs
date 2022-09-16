using System;
using System.ComponentModel.DataAnnotations;

namespace Movies_API.DTO
{
    public class MovieTheaterCreationDTO
    {
        [Required]
        [StringLength(maximumLength: 25)]
        public string Name { get; set; }
        [Range(-90, 90)]
        public double Latitude { get; set; }
        [Range(-180, 190)]
        public double Longitude { get; set; }
    }
}

