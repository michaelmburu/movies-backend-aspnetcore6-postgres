using System;
using AutoMapper;
using Movies_API.DTO.Actor;
using Movies_API.DTO.Genre;
using Movies_API.DTO.MovieTheater;
using Movies_API.Models.Actor;
using Movies_API.Models.Genres;
using Movies_API.Models.MovieTheater;
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
        }
    }
}

