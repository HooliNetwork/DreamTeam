using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Hooli.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Hooli.CloudStorage;
using System.Security.Claims;
using Microsoft.AspNet.Http;
using System.Threading;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Hooli.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        public UserManager<ApplicationUser> UserManager { get; private set; }

        [FromServices]
        public HooliContext DbContext { get; set; }

        [FromServices]
        public Cloud Storage { get; set; }

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileData data, CancellationToken requestAborted, IFormFile file)
        {

            var profileData = await GetCurrentUserAsync();

            profileData.FirstName = user.FirstName;

            profileData.LastName = user.LastName;

            if ((user.DateOfBirth != null) && (user.DateOfBirth.ToString().Length > 0))
            {
                profileData.DateOfBirth = user.DateOfBirth;
            }
            // profileData.RelationshipStatus = user.RelationshipStatus;

            if ((file != null) && (file.Length > 0))
            {
                profileData.ProfilePicture = await Storage.GetUri("profilepictures", Guid.NewGuid().ToString(), file);
            }

            await DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            return View();
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.GetUserId());
        }
    }
}
