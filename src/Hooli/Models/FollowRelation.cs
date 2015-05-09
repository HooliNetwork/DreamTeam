using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.Models
{
    public class FollowRelation
    {
        public string FollowerId { get; set; }
        public string FollowingId { get; set; }
        public virtual ApplicationUser Follower { get; set; }
        public virtual ApplicationUser Following { get; set; }
    }
}
