using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsStacksAPI.Models.Dto
{
    public class PublisherTagDto
    {
        [Required]
        public List<TagDto> Tags { get; set; }
    }
}
