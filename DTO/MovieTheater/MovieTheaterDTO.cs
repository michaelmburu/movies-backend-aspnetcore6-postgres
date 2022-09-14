﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Movies_API.DTO.MovieTheater
{
    public class MovieTheaterDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}

