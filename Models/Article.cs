using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsStacksAPI.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Headline { get; set; }
        public string Description { get; set; }

        [Required]
        public string Body { get; set; }
        public string MetaData { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int CreatedBy { get; set; }

        public DateTime? LastModified { get; set; }
        public int? LastModifiedBy { get; set; }

        public DateTime? PublishedAt { get; set; }
        public int? PublishedBy { get; set; }

        public DateTime? SubmittedAt { get; set; }
        public int? SubmittedBy { get; set; }


        public bool? IsPublished { get; set; }
        public bool? IsSubmitted { get; set; }

        public List<Tag> Tags { get; set; }

        public List<Writer> Writers { get; set; }
        public List<Publisher> Publishers { get; set; }

    }
}
