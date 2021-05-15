using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsStacksAPI.Models.Dto
{
    public class TagDto
    {
        [Required]
        public string Title { get; set; }
    }
}
