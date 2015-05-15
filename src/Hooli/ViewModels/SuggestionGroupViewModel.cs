using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hooli.Models;

namespace Hooli.ViewModels
{
    public class SuggestionGroupViewModel
    {
        public List<Group> Groups { get; set; }
        public List<ApplicationUser> People { get; set; }
        public bool ShowInteresting { get; set; }
    }
}
