using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.Models
{
    public class Group
    {
        [Key]
        public string GroupId { get; set; }

        [Required]
        [StringLength(160, MinimumLength = 2)]
        public string GroupName { get; set; }

        public string Description { get; set; }
        public bool Private {get; set;}

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        [Display(Name = "ImageURI")]
        [StringLength(1024)]
        public string Image { get; set; }

        public virtual List<GroupMember> Members { get; set; }
        //public virtual List<ApplicationUser> BannedUsers { get; set; }
        public virtual List<Post> Posts { get; set; }

        public Group()
        {
            this.DateCreated = DateTime.UtcNow;
        }
    }
}
