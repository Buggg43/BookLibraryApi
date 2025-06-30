using System.ComponentModel.DataAnnotations;

namespace BookLibraryApi.Models
{
    public class BookCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(100)]
        public string Author { get; set; }
        [Required]
        public DateTime Year { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }


    }
}
