using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.Models
{
    public class PostViewModel
    {
       public int? Seed { get; set; }
       public Post post { get; set; }
       public List<string> FollowingPerson { get; set; }
       public List<Post> Children { get; set; }
       public List<string> JoinedGroup { get; set; }
       public string UserId { get; set; }
    }
}
