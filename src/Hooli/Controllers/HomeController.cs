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

        

        public IActionResult About()
        {
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

                return View(model);
            }
            else
            {
                return View(model);
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
                var post = new List<Post>();
                var following = new List<string>();
                if (groupId.Equals("Front"))
                {
                    following = DbContext.FollowRelations
                    .Where(u => u.FollowerId == user.Id)
                    .Select(u => u.FollowingId).ToList();
                    if (latestPosts && following != null)
                    {
                        post = await GetLatestPost(following);
                        return PartialView(post);
                    }
                    else if (following != null)
                    {
                        post = await GetPopularPosts(following);
                        return PartialView(post);
                    }
                    else
                    {
                        post = new List<Post> { new Post() { Title = "No posts!", UserId = user.Id } };
                    }

                }
                else
                {
                    // groupId == UserId because viewing the Profile feed
                    following.Add(groupId);
                    if (latestPosts && following != null)
                    {
                        post = await GetLatestPostProfile(following);
                        return PartialView(post);
                    }
                    else if (following != null)
                    {
                        post = await GetPopularPostsProfile(following);
                        return PartialView(post);
                    }
                    else
                    {
                        post = new List<Post> { new Post() { Title = "No posts!", UserId = user.Id } };
                    }
                }
                return View(post);
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

        private async Task<List<Post>> GetLatestPostProfile(IEnumerable<string> following)
        {
            Console.WriteLine("1");
            var latestPost = await DbContext.Posts
                .OrderByDescending(a => a.DateCreated)
                .Where(a => a.ParentPostId == null)
                .Where(u => following.Contains(u.UserId))
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

        private async Task<List<Post>> GetPopularPostsProfile(IEnumerable<string> following)
        {
            Console.WriteLine("2");
            var postsByVotes = await DbContext.Posts
                .Where(a => a.ParentPostId == null)
                .Where(a => following.Contains(a.UserId))
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
