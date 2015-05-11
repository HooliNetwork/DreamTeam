using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Hooli.Models
{
    public class Image
    {
        public int ImageId { get; set; }
        [StringLength(255)]
        public string ImageName { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
