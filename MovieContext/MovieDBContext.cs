using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Movies_API.Models.Actor;
using Movies_API.Models.Genres;
using Movies_API.Models.MovieTheater;

namespace Movies_API.MovieContext
{
    public class MovieDBContext : DbContext
    {
        public MovieDBContext(DbContextOptions<MovieDBContext> options): base(options)
        {

        }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Actor> Actors { get; set; }

        public DbSet<MovieTheater> MovieTheaters { get; set; }

    }
}

