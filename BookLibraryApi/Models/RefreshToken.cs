using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibraryApi.Models
{
    [Index(nameof(Token), IsUnique = true)] // Move the Index attribute to the class level  
    public class RefreshToken
    {
        public int Id { get; set; }

        [Required]
        public string Token { get; set; }

        public DateTime Expires { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }

        public bool isRevoked { get; set; }
    }
}
