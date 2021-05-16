using System;
using System.Collections.Generic;

namespace NewsStacksAPI.Models.Dto
{
    public class ReaderDto
    {
        public string Headline { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public DateTime? LastModified { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}
