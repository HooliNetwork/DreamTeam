using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        [StringLength(160, MinimumLength = 2)]
        public string Title { get; set; }

        public string Text { get; set; }

        [Display(Name = "Image URL")]
        [StringLength(1024)]
        public string ImgUrl { get; set; }

        public int UpVotes { get; set; }
        public int DownVotes { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }
    }
}
