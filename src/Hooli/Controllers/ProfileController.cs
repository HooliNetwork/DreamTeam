using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Hooli.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Hooli.CloudStorage;
using Hooli.Services;
using System.Security.Claims;
using Microsoft.AspNet.Http;
using System.Threading;
using Hooli.ViewModels;


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

        [FromServices]
        public UserService UserInfo { get; set;}

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<JsonResult> EditProfile([FromForm]EditProfileData data, CancellationToken requestAborted)
        //{

        //    var profileData = await GetCurrentUserAsync();

        //    if ((data.FirstName != null) && (data.FirstName.ToString().Length > 0))
        //    {
        //        profileData.FirstName = data.FirstName;
        //    }

        //    if ((data.LastName != null) && (data.LastName.ToString().Length > 0))
        //    {
        //        profileData.LastName = data.LastName;
        //    }

        //    if ((data.DateOfBirth != null) && (data.DateOfBirth.ToString().Length > 0))
        //    {
        //        profileData.DateOfBirth = data.DateOfBirth;
        //    }

        //    await DbContext.SaveChangesAsync(requestAborted);
        //    return Json("error-not-fully-implemented");
        //}

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileData data)
        {
            Console.WriteLine("Inside EditProfile");
            var user = await GetCurrentUserAsync();
            if((data.FirstName != null) && (data.FirstName.Length > 0))
            {
                user.FirstName = data.FirstName;
            }
            if((data.LastName != null) && (data.LastName.Length > 0))
            {
                user.LastName = data.LastName;
            }
            if (data.DateOfBirth != null)
            {
                user.DateOfBirth = data.DateOfBirth;
            }
            data.Age = await UserInfo.GetAge(user.Id);    

            await DbContext.SaveChangesAsync();
            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfilePicture(IFormFile file, CancellationToken requestAborted)
        {
            var user = await GetCurrentUserAsync();

            if ((file != null) && (file.Length > 0))
            {
                Console.WriteLine("Upload new picture");
                user.ProfilePicture = await Storage.GetUri("profileimages", Guid.NewGuid().ToString(), file);
            }
            await DbContext.SaveChangesAsync(requestAborted);
            return RedirectToAction("Owner");
        }

        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            var currentUser = await GetCurrentUserAsync();
            var user = await DbContext.Users.SingleAsync(u => u.UserName == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            if (user.Id == currentUser.Id)
            {
                return RedirectToAction("Owner");
            }

            var following = DbContext.FollowRelations
                            .Where(u => u.FollowerId == currentUser.Id)
                            .Select(u => u.FollowingId).ToList();
            var isFollowing = following.Contains(user.Id) ? true : false;

            var profileViewModel = new ProfileViewModel()
            {
                User = user,
                Following = isFollowing,
            };

            return View(profileViewModel);
        }

        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Owner()
        {
            var currentUser = await GetCurrentUserAsync();
            var profileViewModel = new ProfileViewModel()
            {
                User = currentUser
            };
            return View(profileViewModel);
        }


        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.GetUserId());
        }
    }
}
