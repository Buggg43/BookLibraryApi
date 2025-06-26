using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace BookLibraryApi.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        [SwaggerSchema(Description = "Tytuł książki")]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        [MaxLength(100)]
        public string Author { get; set; }
        public DateTime Year { get; set; }
        public bool isRead { get; set; }
        public bool isFavorite { get; set; }
    }
}
