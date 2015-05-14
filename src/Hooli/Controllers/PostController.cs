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

        public PostController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;

        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

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
            var currentuser = await GetCurrentUserAsync();
            //dynamic model = new ExpandoObject();
            System.Diagnostics.Debug.WriteLine(id);
            //IEnumerable<Post> posts = postCache.GetHierarchy(id);
            //model.posts = posts;
            var post = RecursiveLoad(id);

            var joined = await DbContext.GroupMembers
                                .Where(u => u.UserId == currentuser.Id)
                                ?.Select(u => u.GroupId).ToListAsync();
            var following = await DbContext.FollowRelations
                                    .Where(u => u.FollowerId == currentuser.Id)
                                    .Select(u => u.FollowingId).ToListAsync();

            PostViewModel model = new PostViewModel { Seed = post.PostId, post = post, JoinedGroup = joined, Children = post.Children, FollowingPerson = following };
            return View(model);
        }


        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, CancellationToken requestAborted, IFormFile file, string id)
        {

            var user = await GetCurrentUserAsync();

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
                    UserName = user.FirstName,
                    UserId = user.Id,
                    Image = post.Image,
                    Link = post.Link,
                    DateCreated = post.DateCreated.ToString("MMM dd, yyy @ HH:mm")
                };
                var following = DbContext.FollowRelations
                        .Where(u => u.FollowingId == user.Id)
                        .Select(u => u.FollowerId)
                        .ToList();
                var usernames = DbContext.Users
                        .Where(u => following.Contains(u.Id))
                        .Select(u => u.UserName).ToList();


                _feedHub.Clients.Users(usernames).feed(postdata);
                //_feedHub.Clients.All.feed(postdata);

                Cache.Remove("latestPost");

                return Redirect("Post/Index/" + post.PostId);
            }
            return View(post);
        }
        [HttpPost]
        public async Task<IActionResult> CreateComment(PostData post)
        {

            return Json(new { responseText = "Success!" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfilePost(Post post, CancellationToken requestAborted, IFormFile file)
        {
            var user = await GetCurrentUserAsync();
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

                return Redirect("/Profile/Owner");
            }
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Vote(string upDown, int postId)
        {
            Console.WriteLine(upDown + postId);
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

        public ActionResult Edit(int id)
        {
            Console.WriteLine("id:" + id);
            var post = DbContext.Posts.Single(p => p.PostId == id);
            if (post == null)
            {
                return HttpNotFound();
            }
            Console.WriteLine("post: " + post.PostId + " " + post.Title + " " + post.Text);
            return View(post);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Post post, CancellationToken requestAborted)
        {


            Console.WriteLine("post: " + post.PostId + " " + post.Title + " " + post.Text);


            if (post != null)
            {
                DbContext.Entry(post).State = EntityState.Modified;
            await DbContext.SaveChangesAsync(requestAborted);
            }

            return Redirect("/Post/Index/" + post.PostId);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken requestAborted)
        {
            var post = DbContext.Posts.Single(p => p.PostId == id);

            Console.WriteLine("Before delete post: " + post.PostId + " " + post.Title + " " + post.Text);

            if (post.ParentPostId == null)
        {
                DbContext.Entry(post).State = EntityState.Deleted;
            await DbContext.SaveChangesAsync(requestAborted);
            }
            else
            {
                Console.WriteLine("Can't delete like above because of ParentPostId");
            }

            //TODO Check if the post has a voteRelation

            Console.WriteLine("After delete post: " + post.PostId + " " + post.Title + " " + post.Text);

            return Redirect("/Group/SingleGroup" + post.GroupGroupId);
        }


        // GET: /StoreManager/Create
        public IActionResult Create()
        {
            return View();
        }
        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.GetUserId());
        }
        private Post RecursiveLoad(int id)
        {
            var ParentFromDatabase = DbContext.Posts
                .Include(u => u.User)
                .Include(g => g.Group)
                .Include(c => c.Children)
                .Single(p => p.PostId == id);

            foreach (var child in ParentFromDatabase.Children)
            {
                var childNotLoaded = child;
                var childFullyLoaded = DbContext.Posts
                  .Include(u => u.User)
                  .Include(d => d.ParentPost)
                  .Include(d => d.Children)
                  .Single(d => d.PostId == childNotLoaded.PostId);

                child.User = childFullyLoaded.User;   
                child.ParentPost = RecursiveLoad(childFullyLoaded.PostId); //Require to set back the value because we want by reference to have everything in the tree
            }
            return ParentFromDatabase;
        }


    }
}
