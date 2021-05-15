using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsStacksAPI.Models.Dto
{
    public class ArticleWriterDto
    {
        [Required]
        public string Headline { get; set; }
        public string Description { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public List<Tag> Tags { get; set; }
    }
}
