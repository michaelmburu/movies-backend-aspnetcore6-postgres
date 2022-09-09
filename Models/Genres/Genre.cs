using System;
using System.ComponentModel.DataAnnotations;
using Movies_API.Validations;

namespace Movies_API.Models.Genres
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="This is required")]
        [StringLength(50)]
        [FirstLetterUppercase] // Attribute validation
        public string Name { get; set; }
    }
}

