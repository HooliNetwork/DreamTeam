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
using Hooli.Services;

namespace Hooli.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [FromServices]
        public HooliContext DbContext { get; set; }

        [FromServices]
        public UserService UserService { get; set; }

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
            var userId = Context.User.GetUserId();
            var people = await UserService.GetFollowedPeople(6, userId);
            var groups = await UserService.GetFollowedGroups(6, userId);
            var interestingGroups = false;
            var interestingPeople = false;
            if (groups.Count() < 6)
            {
                groups = await UserService.GetInterestingGroups(6, userId);
                interestingGroups = true;
            }
            if (people.Count() < 6)
            {
                people = await UserService.GetInterestingPeople(6, userId);
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

        [HttpPost]
        public async Task<IActionResult> Search(string searchString)
        {
            dynamic model = new ExpandoObject();

            if (!String.IsNullOrEmpty(searchString))
            {

                var userId = Context.User.GetUserId();

                model.Users = await DbContext.Users.Where(s => s.LastName
                                            .Contains(searchString)
                                            || s.FirstName.Contains(searchString))
                                            .ToListAsync();

                model.Groups = await DbContext.Groups.Where(g => g.GroupName
                                            .Contains(searchString))
                                            .ToListAsync();

                model.Following = await UserService.GetFollowedPeopleIds(userId);
                model.Joined = await UserService.GetFollowedGroupsIds(userId);

                return View(model);
            }
            else
            {
                return View(model);
            }
        }

        public async Task<IActionResult> Sort(bool latestPosts, bool group, string groupId)
        {
            var user = await UserService.GetUser(Context.User.GetUserId());
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
                    following = await UserService.GetFollowedPeopleIds(user.Id);
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
            var latestPost = await DbContext.Posts
                .OrderByDescending(a => a.DateCreated)
                .Where(a => a.ParentPostId == null)
                .Where(u => (following.Contains(u.UserId)) || (u.UserId == Context.User.GetUserId()))
                .Where(a => (a.DateCreated - DateTime.UtcNow).TotalDays <= 2)
                .Include(u => u.User)
                .ToListAsync();

            return latestPost;
        }

        private async Task<List<Post>> GetLatestPostProfile(IEnumerable<string> following)
        {
            var latestPost = await DbContext.Posts
                .OrderByDescending(a => a.DateCreated)
                .Where(a => a.ParentPostId == null)
                .Where(u => following.Contains(u.UserId))
                .Where(a => (a.DateCreated - DateTime.UtcNow).TotalDays <= 2)
                .Include(u => u.User)
                .ToListAsync();

            return latestPost;
        }

        private async Task<List<Post>> GetPopularPosts(IEnumerable<string> following)
        {
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
            var latestPost = await DbContext.Posts
                .OrderByDescending(a => a.DateCreated)
                .Where(a => a.ParentPostId == null)
                .Where(g => group.Contains(g.GroupGroupId))
                .Where(a => (a.DateCreated - DateTime.UtcNow).TotalDays <= 2)
                .Include(u => u.User)
                .ToListAsync();

            return latestPost;
        }

        private async Task<List<Post>> GetPopularGroupPosts(IEnumerable<string> group)
        {
            var postsByVotes = await DbContext.Posts
                .OrderByDescending(a => a.Points)
                .Where(a => a.ParentPostId == null)
                .Where(g => group.Contains(g.GroupGroupId))
                .Include(u => u.User)
                .ToListAsync();

            return postsByVotes;
        }
    }
}
