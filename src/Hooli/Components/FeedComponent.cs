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
using Hooli.Services;

namespace Hooli.Components
{
    [ViewComponent(Name = "Feed")]
    public class FeedComponent : ViewComponent
    {
        public FeedComponent(UserService userService, HooliContext dbContext)
        {
            UserService = userService;
            DbContext = dbContext;
        }
        public HooliContext DbContext { get;  set; }
        public UserService UserService { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(bool latestPosts, bool group, string groupId)
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
                    return View(post);
                }
                else if (groups != null)
                {
                    var post = await GetPopularGroupPosts(groups);
                    return View(post);
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
                        return View(post);
                    }
                    else if (following != null)
                    {
                        post = await GetPopularPosts(following);
                        return View(post);
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
                        return View(post);
                    }
                    else if (following != null)
                    {
                        post = await GetPopularPostsProfile(following);
                        return View(post);
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
