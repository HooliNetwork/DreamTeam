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
        public string GetAge()
        {
            if(User != null)
            {
                DateTime today = DateTime.Today;
                int age = today.Year - User.DateOfBirth.Year;
                if (User.DateOfBirth > today.AddYears(-age)) age--;
                return age.ToString();
            }
            return null;
        } 
    }
}
