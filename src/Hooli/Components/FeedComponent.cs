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
            System.Diagnostics.Debug.WriteLine("Inside Feed InvokeAsync");
            var user = await GetCurrentUserAsync();
            if (group)
            {
                //System.Diagnostics.Debug.WriteLine(groupId);
                //var groups = user.GroupsMember?.Select(g => g.GroupId);

                var groups = DbContext.GroupMembers
                     .Where(u => u.UserId == user.Id)
                     .Select(u => u.GroupId).ToList();
                System.Diagnostics.Debug.WriteLine("2");
                 if (latestPosts && groups != null)
                { 
                //   var post = await Cache.GetOrSet("latestGroupPost", async context =>
                //  {
                //    context.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                //   return await GetLatestGroupPost(groups);
                //});
                System.Diagnostics.Debug.WriteLine("3");
                var post = GetLatestGroupPost(groups);
                return View(post);
                }
                else if(groups != null)
                {
                    var post = await Cache.GetOrSet("popularGroupPost", async context =>
                    {
                        context.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                        return await GetPopularGroupPosts(groups);
                    });

                    return View(post);
                }
                else
                {
                    return View(new List<Post> { new Post() { Title = "No posts!", User = user} });
                }
            }
            else
            {
                var following = DbContext.FollowRelations
                    .Where(u => u.FollowerId == user.Id)
                    .Select(u => u.FollowingId).ToList();
                //foreach (object o in following)
                //{
                //    System.Diagnostics.Debug.WriteLine(o);
                //}
                if (latestPosts && following != null)
                {
                    var post = await GetLatestPost(following);
                    return View(post);
                }
                else if(following != null)
                {
                    //var post = await Cache.GetOrSet("popularPost", async context =>
                    //{
                        //context.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                         var post = await GetPopularPosts(following);
                    //});
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
                .Where(a => a.ParentPostId == null)
                .Where(g => group.Contains(g.Group.GroupId))
                .Where(a => (a.DateCreated - DateTime.UtcNow).TotalDays <= 2)
                .OrderByDescending(a => a.DateCreated)
                .Include(u => u.User)
                .ToListAsync();

            return latestPost;
        }

        private async Task<List<Post>> GetPopularGroupPosts(IEnumerable<string> group)
        {
            Console.WriteLine("4");

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
