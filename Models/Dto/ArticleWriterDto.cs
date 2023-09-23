using System.ComponentModel.DataAnnotations;

namespace NewsStacksAPI.Models.Dto
{
    public class ArticleWriterDto
    {
        [Required]
        public string Headline { get; set; }
        public string Description { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
