using Hooli.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.ViewModels
{
    public class HomeViewModel
    {
        public List<ApplicationUser> People { get; set; }
        public List<Group> Groups { get; set; }
        public bool InterestingPeople { get; set; }
        public bool InterestingGroups { get; set; }
    }
}
