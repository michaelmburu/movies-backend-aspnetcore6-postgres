using System;
using AutoMapper;
using Movies_API.DTO.Genre;
using Movies_API.Models.Genres;

namespace Movies_API.AutoMapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Genre Mapping
            CreateMap<GenreDTO, Genre>().ReverseMap(); // Allow both way mapping
            CreateMap<GenreCreationDTO, Genre>();
        }
    }
}

