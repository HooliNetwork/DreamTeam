using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.Models
{
    public class VoteRelation
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
        public virtual Post Post { get; set; }
        public virtual ApplicationUser Voter { get; set; }
    }
}
