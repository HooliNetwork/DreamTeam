using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Hooli.Models;
using Microsoft.Framework.Caching.Memory;
using System.Threading;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.Data.Entity;

namespace Hooli.Controllers
{
    public class HomeController : Controller
    {
        [FromServices]
        public HooliContext DbContext { get; set; }
        public HomeController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }
        public UserManager<ApplicationUser> UserManager { get; private set; }

        [FromServices]
        public IMemoryCache Cache { get; set; }
        //
        // GET: /Home/
        public async Task<IActionResult> Index()
        {
            // Get most popular Posts
            var Posts = await Cache.GetOrSet("top", async context =>
            {
                //Refresh it every 10 minutes. Let this be the last item to be removed by cache if cache GC kicks in.
                context.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                context.SetPriority(CachePreservationPriority.High);
                return await GetTopPost(4);
            });
            return View(Posts);
        }

        //Can be removed and handled when HandleError filter is implemented
        //https://github.com/aspnet/Mvc/issues/623
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        public IActionResult StatusCodePage()
        {
            return View("~/Views/Shared/StatusCodePage.cshtml");
        }

        

        public async Task<IActionResult> About()
        {
            // Get most popular Posts
            var Users = await Cache.GetOrSet("users", async context =>
            {
                //Refresh it every 10 minutes. Let this be the last item to be removed by cache if cache GC kicks in.
                context.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                context.SetPriority(CachePreservationPriority.High);
                return await GetUsers(4);
            });
            return View(Users);
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private async Task<List<Post>> GetTopPost(int count)
        {
            // Group the order details by Post and return
            // the Posts with the highest count of Upvotes

            return await DbContext.Post
                .OrderByDescending(a => a.UpVotes)
                .Take(count)
                .Include(u => u.User)
                .ToListAsync();
        }
        private async Task<List<ApplicationUser>> GetUsers(int count)
        {
            // Group the order details by Post and return
            // the Posts with the highest count of Upvotes

            return await DbContext.Users
                .OrderByDescending(a => a.LastName)
                .Take(count)
                .Include(u => u.Posts)
                .ToListAsync();
        }
        


    }
}
