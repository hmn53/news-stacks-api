using System.ComponentModel.DataAnnotations;

namespace NewsStacksAPI.Models.Dto
{
    public class ArticleWriterUpdateDto
    {
        #nullable enable
        public string? Headline { get; set; }
        public string? Description { get; set; }
        public string? Body { get; set; }
    }
}
