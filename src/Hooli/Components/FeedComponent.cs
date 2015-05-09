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
        public HooliContext DbContext
        {
            get;
            set;
        }

        [Activate]
        public IMemoryCache Cache
        {
            get;
            set;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var latestPost = await Cache.GetOrSet("latestPost", async context =>
            {
                context.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                return await GetLatestPost();
            });

            return View(latestPost);
        }

        private Task<Post> GetLatestPost()
        {
            var user = await GetCurrentUserAsync();
            var following = user.Following.Select(c => c.FollowingId);
            var latestPost = await DbContext.Posts
                .OrderByDescending(a => a.DateCreated)
                .Where(a => (a.DateCreated - DateTime.UtcNow).TotalDays <= 2)
                .Where(u => (following.Contains(u.User.Id)))
                .FirstOrDefaultAsync();

            return latestPost;
        }

        private Task<Post> GetPostsOrderedByUpvotes()
        {
            var postsByUpvotes = DbContext.Posts
                .OrderByDescending(a => a.UpVotes - a.DownVotes)
                .Where(a => a.ParentPostId == 0)
                .FirstOrDefaultAsync();

            return postsByUpvotes;
        }

        private Task<Post> GetUserGroups()
        {
            
            return null;
        }


        private Task<Post> GetUserEvents()
        {
            // To do
            return null;
        }
        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.GetUserId());
        }
    }
}
