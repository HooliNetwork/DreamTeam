using Hooli.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.ViewModels
{
    public class ProfileViewModel
    {
        public ApplicationUser User { get; set; }
        public bool Following { get; set; } 
    }
}
