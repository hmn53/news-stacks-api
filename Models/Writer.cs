using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsStacksAPI.Models
{
    public class Writer
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public Account Account { get; set; }

        public List<Article> Articles { get; set; }
    }
}
