using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Framework.Caching.Memory;
using Hooli.Hubs;
using Hooli.Models;
using Hooli.Services;
using Hooli.ViewModels;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.AspNet.Http;
using Microsoft.Net.Http.Headers;
using System.IO;
using Hooli.CloudStorage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.Data.Entity;

namespace Hooli.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private IConnectionManager _connectionManager;
        private IHubContext _feedHub;

        [FromServices]
        public UserService UserService { get; set; }

        [FromServices]
        public PostService postService { get; set; }

        [FromServices]
        public HooliContext DbContext { get; set; }

        [FromServices]
        public Cloud Storage { get; set; }

        [FromServices]
        public IMemoryCache Cache { get; set; }

        [FromServices]
        public IConnectionManager ConnectionManager
        {
            get
            {
                return _connectionManager;
            }
            set
            {
                _connectionManager = value;
                _feedHub = _connectionManager.GetHubContext<FeedHub>();
            }
        }



        public async Task<IActionResult> Index(int id)
        {
            var userId = Context.User.GetUserId();
            var post = postService.FromKey(id);
            var joined = await UserService.GetFollowedGroupsIds(userId);
            var following = await UserService.GetFollowedPeopleIds(userId);

            PostViewModel model = new PostViewModel { Seed = post.PostId, post = post, JoinedGroup = joined, Children = post.Children, FollowingPerson = following, UserId = userId};

            return View(model);
        }


        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, CancellationToken requestAborted, IFormFile file, string id)
        {

            var user = await UserService.GetUser(Context.User.GetUserId());

            // The member has to be in the group to be able to post
            var memberInGroup = DbContext.GroupMembers
                    .Where(u => u.UserId == user.Id)
                    .Where(u => u.GroupId == id);

            if (ModelState.IsValid && user != null && memberInGroup != null)
            {
                post.User = user;
                if ((file != null) && (file.Length > 0))
                {               
                    post.Image = await Storage.GetUri("postimages", Guid.NewGuid().ToString(), file);
                }
                post.GroupGroupId = id;
                DbContext.Posts.Add(post);
                await DbContext.SaveChangesAsync(requestAborted);
                var postdata = new PostData
                {
                    Title = post.Title,
                    Text = post.Text,
                    PostId = post.PostId,
                    Points = post.Points,
                    Username = user.UserName,
                    UserId = user.Id,
                    Image = post.Image,
                    FullName = user.LastName + " " + user.LastName,
                    Link = post.Link,
                    GroupId = post.GroupGroupId,
                    DateCreated = post.DateCreated.ToString("MMM dd, yyy @ HH:mm")
                };

                var following = await UserService.GetFollowingPeopleIds(user.Id);
                var usernames = DbContext.Users
                        .Where(u => following.Contains(u.Id))
                        .Select(u => u.UserName).ToList();

                _feedHub.Clients.Users(usernames).feed(postdata);

                return Redirect("Post/Index/" + post.PostId);
            }
            return View(post);
        }

        [HttpPost]
        public async Task<PostData> CreateComment(PostData post)
        {
            var user = await UserService.GetUser(Context.User.GetUserId());

            if (ModelState.IsValid && user != null)
            {

                var newPost = new Post
                {
                    Text = post.Text,
                    User = user,
                    ParentPostId = post.ParentId,
                    GroupGroupId = post.GroupId,
                };
                post.UserImage = user.ProfilePicture;
                post.Username = user.UserName;
                post.UserId = user.Id;
                post.FullName = user.FirstName + " " + user.LastName;
                post.DateCreated = DateTime.Now.ToString("MMM dd, yyy @ HH:mm");
                DbContext.Posts.Add(newPost);
                await DbContext.SaveChangesAsync();
                return post;
            }
            return new PostData { };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfilePost(Post post, CancellationToken requestAborted, IFormFile file)
        {
            var user = await UserService.GetUser(Context.User.GetUserId());
            if (ModelState.IsValid && user != null)
            {
                post.User = user;
                if ((file != null) && (file.Length > 0))
                {
                    post.Image = await Storage.GetUri("postimages", Guid.NewGuid().ToString(), file);
                }

                post.GroupGroupId = null;
                DbContext.Posts.Add(post);
                await DbContext.SaveChangesAsync(requestAborted);

                var postdata = new PostData
                {
                    Title = post.Title,
                    Text = post.Text,
                    PostId = post.PostId,
                    Points = post.Points,
                    Username = user.FirstName,
                    UserId = user.Id,
                    Image = post.Image,
                    Link = post.Link,
                    DateCreated = post.DateCreated.ToString("MMM dd, yyy @ HH:mm")
                };
                var following = await UserService.GetFollowingPeopleIds(user.Id);
                var usernames = DbContext.Users
                        .Where(u => following.Contains(u.Id))
                        .Select(u => u.UserName).ToList();


                _feedHub.Clients.Users(usernames).feed(postdata);

                Cache.Remove("latestPost");

                return Redirect("/Profile/Owner");
            }
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Vote(string upDown, int postId)
        {
            var voted = await DbContext.VoteRelations.SingleOrDefaultAsync(v => v.UserId == Context.User.GetUserId()
                                                                              && v.PostId == postId);
            if (voted != null)
            {
                return HttpNotFound(); // Hack to return success = false , which does not work.
            }
            var postData = await DbContext.Posts.SingleAsync(postTable => postTable.PostId == postId);
            if (upDown == "up")
            {
                postData.Points++;
            }
            else
            {
                postData.Points--;
            }
            var VoteRelations = new VoteRelation() { PostId = postId, UserId = Context.User.GetUserId() };
            DbContext.VoteRelations.Add(VoteRelations);
            await DbContext.SaveChangesAsync();
            
            
            return Json(new { responseText = "Success!" });
        }

        [HttpPost]
        public async Task<PostData> Edit(PostData data)
        {
            var user = await UserService.GetUser(Context.User.GetUserId());
            var post = await DbContext.Posts
                        .SingleAsync(p => p.PostId == data.PostId);
            if ((data.Title != null) && (data.Title.Length > 0))
            {
                post.Title = data.Title;
            }
            if ((data.Text != null) && (data.Text.Length > 0))
            {
                post.Text = data.Text;
        }

            await DbContext.SaveChangesAsync();

            return data;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken requestAborted)
        {
            var post =await DbContext.Posts
                .Include(u => u.User)
                .SingleAsync(p => p.PostId == id);
            var voteRelation = await DbContext.VoteRelations
                .Where(u => u.PostId == post.PostId)
                .ToListAsync();
            var groupId = post.GroupGroupId;
            var userName = post.User.UserName;

            // Check if there are any comments and mark them for deletion
            if (post.ParentPostId == null)
            {
                foreach (var p in postService.GetThisAndChild(post.PostId))
                {
                    DbContext.Remove(p);
                }
            }

            // Check if there are any votes on the post and mark the relation for deletion
            if (voteRelation != null)
            {
                foreach (var relationLine in voteRelation)
            {
                   DbContext.Remove(relationLine);
                }
            }
            await DbContext.SaveChangesAsync(requestAborted);


            //Redirecting after the delete based on where the post was based
            if (groupId != null)
        {
                return Redirect("/Group/SingleGroup/" + groupId);
            }
            else if (userName != null)
        {
                return Redirect("/Profile/Index/" + userName);
            }
            else
            {
                return Redirect("/");
            }


        }
        // GET: /StoreManager/Create
        public IActionResult Create()
        {
            return View();
        }

    }
}
