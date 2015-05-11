using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Caching.Memory;
using Hooli.Models;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace Hooli.Components
{
    [ViewComponent(Name = "Feed")]
    public class FeedComponent : ViewComponent
    {
        public FeedComponent(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }
        public UserManager<ApplicationUser> UserManager { get; private set; }

        [Activate]
        public HooliContext DbContext { get; set; }

        [Activate]
        public IMemoryCache Cache { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(bool latestPosts, bool group)
        {
            System.Diagnostics.Debug.WriteLine("Inside Invoke Async");
            var user = await GetCurrentUserAsync();
            System.Diagnostics.Debug.WriteLine("UserID" + user.Id);
            if (group)
            {
                System.Diagnostics.Debug.WriteLine("Inside group");
                var groups = user.Groups?.Select(g => g.GroupId);
                System.Diagnostics.Debug.WriteLine("1");
                if (latestPosts && groups != null)
                {
                    System.Diagnostics.Debug.WriteLine("2");
                    var post = await Cache.GetOrSet("latestGroupPost", async context =>
                    {
                        context.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                        return await GetLatestGroupPost(groups);
                    });
                    System.Diagnostics.Debug.WriteLine("3");
                    return View(post);
                }
                else if(groups != null)
                {
                    System.Diagnostics.Debug.WriteLine("4");
                    var post = await Cache.GetOrSet("popularGroupPost", async context =>
                    {
                        context.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                        return await GetPopularGroupPosts(groups);
                    });
                    System.Diagnostics.Debug.WriteLine("5");
                    return View(post);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("6");
                    System.Diagnostics.Debug.WriteLine("User first name " + user.FirstName);
                    return View(new List<Post> { new Post() { Title = "No posts!", User = user} });
                }
            }
            else
            {
                //var following = user.Followers.Select(c => c.Following.Id);
                var following = DbContext.FollowRelations
                    .Where(u => u.FollowerId == user.Id)
                    .Select(u => u.FollowingId).ToList();
                foreach (object o in following)
                {
                    System.Diagnostics.Debug.WriteLine(o);
                }
                if (latestPosts && following != null)
                {
                    //var post = await Cache.GetOrSet("latestPost", async context =>
                    //{
                    //    context.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                        var post = await GetLatestPost(following);
                    //});
                    return View(post);
                }
                else if(following != null)
                {
                    var post = await Cache.GetOrSet("popularPost", async context =>
                    {
                        context.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                        return await GetPopularPosts(following);
                    });
                    return View(post);
                }
                else
                {
                    return View(new List<Post> { new Post() { Title = "No posts!", UserId = user.Id } });
                }
            }
        }

        private async Task<List<Post>> GetLatestPost(IEnumerable<string> following)
        {
            var latestPost = await DbContext.Posts
                .OrderByDescending(a => a.DateCreated)
                .Where(a => a.ParentPostId == null)
                .Where(u => (following.Contains(u.UserId)))
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
            var postsByVotes = await DbContext.Posts
                .Where(a => a.ParentPostId == null)
                .Where(a => (following.Contains(a.UserId)))
                .OrderByDescending(a => a.Points)
                .Include(u => u.User)
                .ToListAsync();

            return postsByVotes;
        }

        private async Task<List<Post>> GetLatestGroupPost(IEnumerable<int> group)
        {
            var latestPost = await DbContext.Posts
                .Where(a => a.ParentPostId == null)
                .Where(g => group.Contains(g.Group.GroupId))
                .Where(a => (a.DateCreated - DateTime.UtcNow).TotalDays <= 2)
                .OrderByDescending(a => a.DateCreated)
                .Include(u => u.User)
                .ToListAsync();

            return latestPost;
        }

        private async Task<List<Post>> GetPopularGroupPosts(IEnumerable<int> group)
        {
            var postsByVotes = await DbContext.Posts
                .Where(a => a.ParentPostId == null)
                .Where(g => group.Contains(g.Group.GroupId))
                .OrderByDescending(a => a.Points)
                .Include(u => u.User)
                .ToListAsync();

            return postsByVotes;
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.GetUserId());
        }
    }
}
