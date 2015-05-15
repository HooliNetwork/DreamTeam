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

        [FromServices]
        public HooliContext DbContext { get; set; }

        [FromServices]
        public UserService UserService { get; set; }

        [FromServices]
        public Cloud Storage { get; set; }

        [FromServices]
        public UserService UserInfo { get; set;}

        [HttpPost]
        public async Task<EditProfileData> EditProfile(EditProfileData data)
        {
            var user = await UserService.GetUser(Context.User.GetUserId());
            if ((data.FirstName != null) && (data.FirstName.Length > 0))
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
            return data;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfilePicture(IFormFile file, CancellationToken requestAborted)
        {
            var user = await UserService.GetUser(Context.User.GetUserId());

            if ((file != null) && (file.Length > 0))
            {
                user.ProfilePicture = await Storage.GetUri("profileimages", Guid.NewGuid().ToString(), file);
            }
            await DbContext.SaveChangesAsync(requestAborted);
            return RedirectToAction("Owner");
        }

        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            var currentUser = await UserService.GetUser(Context.User.GetUserId());
            var user = await DbContext.Users.SingleAsync(u => u.UserName == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            if (user.Id == currentUser.Id)
            {
                return RedirectToAction("Owner");
            }

            var following = await UserService.GetFollowedPeopleIds(currentUser.Id);
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
            var currentUser = await UserService.GetUser(Context.User.GetUserId());
            var profileViewModel = new ProfileViewModel()
            {
                User = currentUser
            };
            return View(profileViewModel);
        }
    }
}
