using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Hooli.Models;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Hooli.Controllers
{
    public class ProfileController : Controller
    {
        [FromServices]
        public HooliContext DbContext { get; set; }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(ApplicationUser user)
        {

            var profileData = DbContext.User.Single(userTable => userTable.Id == user.Id);

            profileData.FirstName = user.FirstName;
            profileData.LastName = user.LastName;
            profileData.DateOfBirth = user.DateOfBirth;
            profileData.RelationshipStatus = user.RelationshipStatus;
            profileData.ProfilePicture = user.ProfilePicture;

            DbContext.SaveChanges(); 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ListUserPostByDate(ApplicationUser user)
        {
            // To do
            return View();
        }
    }
}
