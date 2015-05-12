using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.Models
{
    public class GroupMember
    {
        public string GroupId { get; set; }
        public string UserId { get; set; }
        public bool banned { get; set; }
        public virtual Group Group { get; set; }
        public virtual ApplicationUser Member { get; set; }
    }
}
