using System;
namespace Movies_API.DTO
{
    public class LandingPageDTO
    {
        public List<MovieDTO> InTheaters { get; set; }
        public List<MovieDTO> UpComingReleases { get; set; }
    }
}

