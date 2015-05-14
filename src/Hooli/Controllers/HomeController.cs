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
using Hooli.Components;

namespace Hooli.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [FromServices]
        public HooliContext DbContext { get; set; }

        [FromServices]
        public UserManager<ApplicationUser> UserManager { get; set; }

        public FeedComponent feedComponent { get; set; }
        public HomeController(FeedComponent _feedComponent)
        {
            feedComponent = _feedComponent;
        }

        [FromServices]
        public IMemoryCache Cache { get; set; }

        //
        // GET: /Home/
        public async Task<IActionResult> Index(string sortOrder)
        {
            // Get most popular Posts, from following users
            //var Posts = await Cache.GetOrSet("top", async context =>
            //{
            //    //Refresh it every 10 minutes. Let this be the last item to be removed by cache if cache GC kicks in.
            //    context.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            //    context.SetPriority(CachePreservationPriority.High);
            //    return await GetMostRecentPosts(10);
            //});
            //var user = await GetCurrentUserAsync();
            //user.Followers = DbContext.FollowRelations.Where(f => f.FollowerId == user.Id).ToList();
            //user.Following = DbContext.FollowRelations.Where(f => f.FollowingId == user.Id).ToList();

            return View();
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

        [HttpPost("Search")]
        public async Task<IActionResult> Search(string searchString)
        {
            dynamic model = new ExpandoObject();
            if (!String.IsNullOrEmpty(searchString))
            {
                var currentuser = await GetCurrentUserAsync();
                model.Users = await DbContext.Users.Where(s => s.LastName
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

        //[HttpPost]
        //public async Task<IViewComponentResult> Sort(bool latestPosts, bool group, string groupId)
        //{
        //    //var feedComponent = new FeedComponent(UserManager);
        //    System.Diagnostics.Debug.WriteLine("\n\n\n context \n\n " + Context.User.GetUserId() + "\n\n\n");
            
        //    return await feedComponent.InvokeAsync( latestPosts, group, groupId, Context.User.GetUserId());
        //    //System.Diagnostics.Debug.WriteLine("\n\n\n hello \n\n " + result.ToString() + "\n\n\n");
        //    //return View(result);
        //}

        public async Task<IActionResult> Sort(bool latestPosts, bool group, string groupId)
        {
            System.Diagnostics.Debug.WriteLine("Inside Feed InvokeAsync");
            var user = await GetCurrentUserAsync(); 

            if (group)
            {
                // Create the list of groups to show posts from
                // Either all followed groups or inside a single group
                var groups = new List<string>();
                if (groupId.Equals("Front"))
                {
                    groups = DbContext.GroupMembers
                    .Where(u => u.UserId == user.Id)
                    .Select(u => u.GroupId).ToList();
                }
                else
                {
                    groups.Add(groupId);
                }

                // Check if filtering should show latest posts or popular posts from groups
                if (latestPosts && groups != null)
                {
                    var post = await GetLatestGroupPost(groups);
                    return PartialView(post);
                }
                else if (groups != null)
                {
                    var post = await GetPopularGroupPosts(groups);
                    return PartialView(post);
                }
                else
                {
                    return View(new List<Post> { new Post() { Title = "No posts!", User = user } });
                }
            }
            else
            {
                // Create the list of followers to show from
                // Either all followed users or a owner of a profile
                var following = new List<string>();
                if (groupId.Equals("Front"))
                {
                    following = DbContext.FollowRelations
                    .Where(u => u.FollowerId == user.Id)
                    .Select(u => u.FollowingId).ToList();
                }
                else
                {
                    //Todo something that figures out what user we are looking at
                }

                // Check if filtering should show latest posts or popular posts from users
                if (latestPosts && following != null)
                {
                    var post = await GetLatestPost(following);
                    return PartialView(post);
                }
                else if (following != null)
                {
                    var post = await GetPopularPosts(following);
                    return PartialView(post);
                }
                else
                {
                    return View(new List<Post> { new Post() { Title = "No posts!", UserId = user.Id } });
                }
            }
        }

        private async Task<List<Post>> GetLatestPost(IEnumerable<string> following)
        {
            Console.WriteLine("1");
            var latestPost = await DbContext.Posts
                .OrderByDescending(a => a.DateCreated)
                .Where(a => a.ParentPostId == null)
                .Where(u => (following.Contains(u.UserId)) || (u.UserId == Context.User.GetUserId()))
                .Where(a => (a.DateCreated - DateTime.UtcNow).TotalDays <= 2)
                .Include(u => u.User)
                .ToListAsync();
            foreach (object o in latestPost)
            {
                Console.WriteLine(o);
            }
            return latestPost;
        }

        private async Task<List<Post>> GetPopularPosts(IEnumerable<string> following)
        {
            Console.WriteLine("2");
            var postsByVotes = await DbContext.Posts
                .Where(a => a.ParentPostId == null)
                .Where(a => (following.Contains(a.UserId)) || (a.UserId == Context.User.GetUserId()))
                .OrderByDescending(a => a.Points)
                .Include(u => u.User)
                .ToListAsync();

            return postsByVotes;
        }

        private async Task<List<Post>> GetLatestGroupPost(IEnumerable<string> group)
        {
            Console.WriteLine("3");

            var latestPost = await DbContext.Posts
                .OrderByDescending(a => a.DateCreated)
                .Where(a => a.ParentPostId == null)
                .Where(g => group.Contains(g.GroupGroupId))
                .Where(a => (a.DateCreated - DateTime.UtcNow).TotalDays <= 2)
                .Include(u => u.User)
                .ToListAsync();
            foreach (object o in latestPost)
            {
                Console.WriteLine(o);
            }

            return latestPost;
        }

        private async Task<List<Post>> GetPopularGroupPosts(IEnumerable<string> group)
        {
            Console.WriteLine("4");

            var postsByVotes = await DbContext.Posts
                .OrderByDescending(a => a.Points)
                .Where(a => a.ParentPostId == null)
                .Where(g => group.Contains(g.GroupGroupId))
                .Include(u => u.User)
                .ToListAsync();
            foreach (object o in postsByVotes)
            {
                Console.WriteLine(o);
            }
            return postsByVotes;
        }
    }
}
