using System.ComponentModel.DataAnnotations;

namespace NewsStacksAPI.Models
{
    public class Account
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
