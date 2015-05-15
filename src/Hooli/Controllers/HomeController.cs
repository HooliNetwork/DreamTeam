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
using Microsoft.AspNet.Authorization;
using System.Dynamic;
using Hooli.ViewModels;

namespace Hooli.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [FromServices]
        public HooliContext DbContext { get; set; }

        [FromServices]
        public UserManager<ApplicationUser> UserManager { get; set; }

        [FromServices]
        public IMemoryCache Cache { get; set; }
        //
        // GET: /Home/
        public async Task<IActionResult> Index(string sortOrder)
        {
            var people = await GetFollowedPeople(6);
            var groups = await GetFollowedGroups(6);
            var interestingGroups = false;
            var interestingPeople = false;
            if (groups.Count() < 6)
            {
                groups = await GetOtherGroups(6);
                interestingGroups = true;
            }
            if (people.Count() < 6)
            {
                people = await GetOtherPeople(6);
                interestingPeople = true;
            }
            return View(new HomeViewModel {
                Groups = groups,
                People = people,
                InterestingPeople = interestingPeople,
                InterestingGroups = interestingGroups}
            );

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
            //var Users = await Cache.GetOrSet("users", async context =>
            //{
            //Refresh it every 10 minutes. Let this be the last item to be removed by cache if cache GC kicks in.
            //    context.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            //   context.SetPriority(CachePreservationPriority.High);
            //    return await GetUsers(4);
            //});
            //return View(Users);
            return View();
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

            return await DbContext.Posts
                .OrderByDescending(a => a.Points)
                .Take(count)
                .Include(u => u.User)
                .ToListAsync();
        }
        private async Task<List<Post>> GetMostRecentPosts(int count)
        {
            // Group the order details by Post and return
            // the Posts with the highest count of Upvotes
            var user = await GetCurrentUserAsync();
            var FollowedUsers = user.Following;
            return await DbContext.Posts
                .OrderByDescending(a => a.Points)
                .Take(count)
                .Include(u => u.User)
                .ToListAsync();
        }

        private async Task<List<ApplicationUser>> GetFollowedPeople(int count)
        {
            // Group the order details by Post and return
            // the Posts with the highest count of Upvotes
            var following = await DbContext.FollowRelations
                .Where(u => u.FollowerId == Context.User.GetUserId())
                .Select(u => u.FollowingId).ToListAsync();

            return await DbContext.Users

                .Where(u => following.Contains(u.Id))
                .Take(count)
                .ToListAsync();
        }
        private async Task<List<Group>> GetFollowedGroups(int count)
        {
            // Group the order details by Post and return
            // the Posts with the highest count of Upvotes
            var following = await DbContext.GroupMembers
                .Where(u => u.UserId == Context.User.GetUserId())
                .Select(u => u.GroupId).ToListAsync();

            return await DbContext.Groups
                .Where(u => following.Contains(u.GroupId))
                .Take(count)
                .ToListAsync();
        }
        private async Task<List<ApplicationUser>> GetOtherPeople(int count)
        {
            // Group the order details by Post and return
            // the Posts with the highest count of Upvotes
            var following = await DbContext.FollowRelations
                .Where(u => u.FollowerId == Context.User.GetUserId())
                .Select(u => u.FollowingId).ToListAsync();

            return await DbContext.Users
                .Where(u => !following.Contains(u.Id))
                .Take(count)
                .ToListAsync();
        }
        private async Task<List<Group>> GetOtherGroups(int count)
        {
            // Group the order details by Post and return
            // the Posts with the highest count of Upvotes
            var following = await DbContext.GroupMembers
                .Where(u => u.UserId == Context.User.GetUserId())
                .Select(u => u.GroupId).ToListAsync();

            return await DbContext.Groups
                .Where(u => !following.Contains(u.GroupId))
                .Take(count)
                .ToListAsync();
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(string searchString)
        {
            dynamic model = new ExpandoObject();
            if (!String.IsNullOrEmpty(searchString))
            {
                var currentuser = await GetCurrentUserAsync();
                model.Users =  await DbContext.Users.Where(s => s.LastName
                                            .Contains(searchString)
                                            || s.FirstName.Contains(searchString))
                                            .ToListAsync();

                model.Groups = await DbContext.Groups.Where(g => g.GroupName
                                            .Contains(searchString))
                                            .ToListAsync();

                model.Following = await DbContext.FollowRelations
                                .Where(u => u.FollowerId == currentuser.Id)
                                .Select(u => u.FollowingId).ToListAsync();
                model.Joined = await DbContext.GroupMembers
                                .Where(u => u.UserId == currentuser.Id)
                                .Select(u => u.GroupId).ToListAsync();

    
                //model.Events = View(await DbContext.Events.Where(e => e.EventName
                //                            .Contains(searchString))
                //                            .ToListAsync());
                return View(model);
            }
            else
            {
                return View();
            }
        }
        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.GetUserId());
        }



    }
}
