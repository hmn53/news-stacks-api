using System.ComponentModel.DataAnnotations;

namespace NewsStacksAPI.Models.Dto
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
