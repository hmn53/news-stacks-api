using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsStacksAPI.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public List<Article> Articles { get; set; }
    }
}
