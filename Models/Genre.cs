using System;
using Movies_API.Validations;
using System.ComponentModel.DataAnnotations;

namespace Movies_API.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "This is required")]
        [StringLength(50)]
        [FirstLetterUppercase] // Attribute validation
        public string Name { get; set; }
    }
}

