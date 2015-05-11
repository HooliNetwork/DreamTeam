using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [StringLength(160, MinimumLength = 2)]
        public string EventName { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public byte[] Image { get; set; }

        public bool Private { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public virtual List<ApplicationUser> AttendingUsers { get; set; }
        public virtual List<ApplicationUser> InvitedUsers { get; set; }
        public virtual List<Post> Posts { get; set; }

        public Event()
        {
            this.DateCreated = DateTime.UtcNow;
        }
    }
}
