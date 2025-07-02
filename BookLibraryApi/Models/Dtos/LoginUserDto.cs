using System.ComponentModel.DataAnnotations;

namespace BookLibraryApi.Models.Dtos
{
    public class LoginUserDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Username { get; set; }
        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; set; }
    }
}
