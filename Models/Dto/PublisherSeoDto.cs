using System.ComponentModel.DataAnnotations;

namespace NewsStacksAPI.Models.Dto
{
    public class PublisherSeoDto
    {
        [Required]
        public string MetaData { get; set; }
    }
}
