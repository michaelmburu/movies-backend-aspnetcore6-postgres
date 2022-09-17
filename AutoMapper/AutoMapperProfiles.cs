using System;
using AutoMapper;
using Movies_API.DTO;
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
                .ForMember(x => x.MoviesGenres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MovieTheatersMovies, options => options.MapFrom(MapMovieTheatersMovies))
                .ForMember(x => x.MoviesActors, options => options.MapFrom(MapMoviesActors));

            CreateMap<Movie, MovieDTO>()
                .ForMember(x => x.Genres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MovieTheaters, options => options.MapFrom(MapMovieTheatersMovies))
                .ForMember(x => x.Actors, options => options.MapFrom(MapMoviesActors));
        }

        private List<ActorsMovieDTO> MapMoviesActors(Movie movie, MovieDTO movieDTO)
        {
            var result = new List<ActorsMovieDTO>();

            if(movie.MoviesActors != null)
            {
                foreach(var moviesActors in movie.MoviesActors)
                {
                    result.Add(new ActorsMovieDTO()
                    {
                        Id = moviesActors.ActorId,
                        Name = moviesActors.Actor.Name,
                        Character = moviesActors.Character,
                        Picture = moviesActors.Actor.Picture,
                        Order = moviesActors.Order
                    });
                }
            }

            return result;
        }

        private List<MovieTheaterDTO> MapMovieTheatersMovies(Movie movie, MovieDTO movieDTO)
        {
            var result = new List<MovieTheaterDTO>();

            if (movie.MovieTheatersMovies != null)
            {
                foreach (var movieTheatersMovies in movie.MovieTheatersMovies)
                {
                    result.Add(new MovieTheaterDTO() {
                        Id = movieTheatersMovies.MovieTheaterId,
                        Name = movieTheatersMovies.MovieTheater.Name,
                        Latitude = movieTheatersMovies.MovieTheater.Location.Y,
                        Longitude = movieTheatersMovies.MovieTheater.Location.X,
                    });
                }
            }

            return result;
        }

        private List<GenreDTO> MapMoviesGenres(Movie movie, MovieDTO movieDTO)
        {
            var result = new List<GenreDTO>();

            if (movie.MoviesGenres != null)
            {
                foreach(var genre in movie.MoviesGenres)
                {
                    result.Add(new GenreDTO() { Id = genre.GenreId, Name = genre.Genre.Name });
                }
            }

            return result;
        }

        private List<MoviesGenres> MapMoviesGenres(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesGenres>();

            if(movieCreationDTO.GenresIds == null) { return result; }

            foreach(var id in movieCreationDTO.GenresIds)
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

