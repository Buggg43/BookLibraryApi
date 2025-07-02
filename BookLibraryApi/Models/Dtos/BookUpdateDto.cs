using System.ComponentModel.DataAnnotations;

namespace BookLibraryApi.Models.Dtos
{
    public class BookUpdateDto
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
        public bool isRead { get; set; }
        public bool isFavorite { get; set; }
    }
}
