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

        public async Task<IViewComponentResult> InvokeAsync(bool latestPosts, bool group)
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                if (group)
                {
                    var groups = user.Groups.Select(g => g.GroupId);
           
                    if (latestPosts)
                    {
                        var post = await Cache.GetOrSet("latestGroupPost", async context =>
                        {
                            context.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                            return await GetLatestGroupPost(groups);
                        });
                        return View(post);
                    }
                    else
                    {
                        var post = await Cache.GetOrSet("popularGroupPost", async context =>
                        {
                            context.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                            return await GetPopularGroupPosts(groups);
                        });
                        return View(post);
                    }
                }
                else
                {
                    var following = user.Following.Select(c => c.FollowingId);
                    if (latestPosts)
                    {
                        var post = await Cache.GetOrSet("latestPost", async context =>
                        {
                            context.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                            return await GetLatestPost(following);
                        });
                        return View(post);
                    }
                    else
                    {
                        var post = await Cache.GetOrSet("popularPost", async context =>
                        {
                            context.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                            return await GetPopularPosts(following);
                        });
                        return View(post);
                    }
                }
            }
            else
            {
                return View(new Post { Title = "No posts!" });
            }
        }

        private async Task<List<Post>> GetLatestPost(IEnumerable<string> following)
        {
            var latestPost = await DbContext.Posts
                .Where(a => a.ParentPostId == null)
                .Where(u => (following.Contains(u.User.Id)))
                .OrderByDescending(a => a.DateCreated)
                .Where(a => (a.DateCreated - DateTime.UtcNow).TotalDays <= 2)
                .ToListAsync();

            return latestPost;
        }

        private async Task<List<Post>> GetPopularPosts(IEnumerable<string> following)
        {
            var postsByVotes = await DbContext.Posts
                .Where(a => a.ParentPostId == null)
                .Where(a => (following.Contains(a.User.Id)))
                .OrderByDescending(a => a.UpVotes - a.DownVotes)
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
                .ToListAsync();

            return latestPost;
        }

        private async Task<List<Post>> GetPopularGroupPosts(IEnumerable<int> group)
        {
            var postsByVotes = await DbContext.Posts
                .Where(a => a.ParentPostId == null)
                .Where(g => group.Contains(g.Group.GroupId))
                .OrderByDescending(a => a.UpVotes - a.DownVotes)
                .ToListAsync();

            return postsByVotes;
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.GetUserId());
        }
    }
}
