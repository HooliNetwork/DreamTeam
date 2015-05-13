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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(IFormCollection form, CancellationToken requestAborted)
        {

            var profileData = await GetCurrentUserAsync();

            profileData.FirstName = Convert.ToString(form["first_name"]);
            profileData.LastName = Convert.ToString(form["last_name"]);
            profileData.DateOfBirth = Convert.ToDateTime(form["date_birth"]);



            await DbContext.SaveChangesAsync(requestAborted);
            return Json("error-not-fully-implemented");
        }

        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            var q = Request.QueryString;
            Console.WriteLine(q);    
            Console.WriteLine("Inside index " + id);

            var user = await DbContext.Users.SingleAsync(u => u.UserName == id);
            if(user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }


        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.GetUserId());
        }
    }
}
