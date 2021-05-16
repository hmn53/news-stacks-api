using System.ComponentModel.DataAnnotations;

namespace NewsStacksAPI.Models.Dto
{
    public class TagDto
    {
        [Required]
        public string Title { get; set; }
    }
}
