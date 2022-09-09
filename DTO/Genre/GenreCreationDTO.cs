using System;
using Movies_API.Validations;
using System.ComponentModel.DataAnnotations;

namespace Movies_API.DTO.Genre
{
    public class GenreCreationDTO
    {
        [Required(ErrorMessage = "This is required")]
        [StringLength(50)]
        [FirstLetterUppercase] // Attribute validation
        public string Name { get; set; }
    }
}

