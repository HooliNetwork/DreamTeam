using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using Hooli.Models;
using Hooli.CloudStorage;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Hooli.Controllers
{
    [Authorize]
    public class PeopleController : Controller
    {
        [FromServices]
        public HooliContext DbContext { get; set; }

        [FromServices]
        public Cloud storage { get; set; }

        [FromServices]
        public UserManager<ApplicationUser> UserManager { get; set; }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ViewResult> FollowedList()
        {
            System.Diagnostics.Debug.WriteLine("The followed");
            var user = await GetCurrentUserAsync();
            //A list of followersId
            var people = DbContext.FollowRelations
                .Where(u => u.FollowerId == user.Id)
                .Select(u => u.FollowingId).ToList();
            //A list of users followed
            var peoples = await DbContext.Users
                .Where(p => people.Contains(p.Id))
                .ToListAsync();
            System.Diagnostics.Debug.WriteLine("Followed list function");
            foreach(var o in peoples)
            {
                System.Diagnostics.Debug.WriteLine(o.FirstName);
            }
            return View(peoples);
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.GetUserId());
        }
    }
}
