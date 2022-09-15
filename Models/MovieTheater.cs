using System;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace Movies_API.Models
{
    public class MovieTheater
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 25)]
        public string Name { get; set; }
        public Point Location { get; set; }
    }
}

