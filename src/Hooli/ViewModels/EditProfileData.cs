using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.ViewModels
{
    public class EditProfileData
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Age { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IFormFile ProfilePicture { get; set; }
    }
}
