using System.ComponentModel.DataAnnotations;

namespace BookLibraryApi.Models.Dtos
{
    public class RegisterUserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
