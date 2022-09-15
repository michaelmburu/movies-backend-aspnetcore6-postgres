using System;
using AutoMapper;
using Movies_API.DTO;
using Movies_API.DTO.Actor;
using Movies_API.DTO.Genre;
using Movies_API.DTO.MovieTheater;
using Movies_API.Models;
using NetTopologySuite.Geometries;

namespace Movies_API.AutoMapper
{
    public class AutoMapperProfiles : Profile
    {

        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            // Genre Mapping
            CreateMap<GenreDTO, Genre>().ReverseMap(); // Allow both way mapping
            CreateMap<GenreCreationDTO, Genre>();

            //ActorDTO Mapping
            CreateMap<ActorDTO, Actor>().ReverseMap();
            CreateMap<ActorCreationDTO, Actor>()
                .ForMember(x => x.Picture, options => options.Ignore()); //Ignore picture entity during mapping

            //MovieTheater
            CreateMap<MovieTheater, MovieTheaterDTO>()
                .ForMember(x => x.Latitude, l => l.MapFrom(prop => prop.Location.Y))
                .ForMember(x => x.Longitude, l => l.MapFrom(prop => prop.Location.X));

            CreateMap<MovieTheaterCreationDTO, MovieTheater>()
                .ForMember(x => x.Location, x => x.MapFrom(m =>
                (
                    geometryFactory.CreatePoint(new Coordinate(m.Longitude, m.Latitude)
                ))));

            CreateMap<MovieCreationDTO, Movie>()
              .ForMember(x => x.Poster, options => options.Ignore())
              .ForMember(x => x.MovieGenres, options => options.MapFrom(MapMoviesGenres))
              .ForMember(x => x.MovieTheatersMovies, options => options.MapFrom(MapMovieTheatersMovies))
              .ForMember(x => x.MoviesActors, options => options.MapFrom(MapMoviesActors));
        }

        private object MapMoviesActors(MovieTheaterCreationDTO arg)
        {
            throw new NotImplementedException();
        }

        private List<MoviesGenres> MapMoviesGenres(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesGenres>();

            if(movieCreationDTO.GenreIds == null) { return result; }

            foreach(var id in movieCreationDTO.GenreIds)
            {
                result.Add(new MoviesGenres() { GenreId = id });
            }

            return result;
        }

        private List<MovieTheatersMovies> MapMovieTheatersMovies(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MovieTheatersMovies>();

            if (movieCreationDTO.MovieTheatersIds == null) { return result; }

            foreach (var id in movieCreationDTO.MovieTheatersIds)
            {
                result.Add(new MovieTheatersMovies() {  MovieTheaterId = id });
            }

            return result;
        }

        private List<MoviesActors> MapMoviesActors(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesActors>();

            if (movieCreationDTO.Actors == null) { return result; }

            foreach (var actor in movieCreationDTO.Actors)
            {
                result.Add(new MoviesActors() { ActorId = actor.Id, Character = actor.Character });
            }

            return result;
        }
    }
}

