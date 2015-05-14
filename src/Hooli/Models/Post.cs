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

        [Display(Name = "Link")]
        [StringLength(1024)]
        public string Link { get; set; }

        [Display(Name = "ImageURI")]
        [StringLength(1024)]
        public string Image { get; set; }

        public int Points { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateCreated { get; set; }

        public virtual Group Group { get; set; }

        public int? ParentPostId { get; set; }
        public Post ParentPost { get; set; }
        public List<Post> Children { get; set; }
        public string UserId { get; set; }

        public string GroupGroupId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public Post()
        {
            this.DateCreated = DateTime.UtcNow;
            Points = 0;
        }
    }
}