using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        [Display(Name = "Image URL")]
        [StringLength(1024)]
        public string ImgUrl { get; set; }

        public int Points { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        public virtual Group Group { get; set; }

        public int? ParentPostId { get; set; }
        public Post ParentPost { get; set; }
        public List<Post> Children { get; set; }

        public virtual ApplicationUser User { get; set; }

        public Post()
        {
            this.DateCreated = DateTime.UtcNow;
            Points = 0;
        }
    }
}
