using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibraryApi.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        public bool isRevoked { get; set; }
    }
}
