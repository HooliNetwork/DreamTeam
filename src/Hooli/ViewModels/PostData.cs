using Hooli.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.ViewModels
{
    public class PostData
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public int Points { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Image { get; set; }
        public string DateCreated { get; set; }
        public string Link { get; set; }
        public int ParentId { get; set; }
    }
}
