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
        [Display(Name = "Unique name")]
<<<<<<< HEAD
        public string GroupId { get; set; }
=======
        public int GroupId { get; set; }
>>>>>>> Working on creating a group, not yet possible to save to database.

        [Required]
        [StringLength(160, MinimumLength = 2)]
        [Display(Name = "Group name")]
        public string GroupName { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
        public bool Private {get; set;}

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        public byte[] Image { get; set; }

        public virtual List<GroupMember> Members { get; set; }
        //public virtual List<ApplicationUser> BannedUsers { get; set; }
        public virtual List<Post> Posts { get; set; }

        public Group()
        {
            this.DateCreated = DateTime.UtcNow;
        }
    }
}
