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

namespace Hooli.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private IConnectionManager _connectionManager;
        private IHubContext _feedHub;

        [FromServices]
        public PostCache postCache { get; set; }

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

            Console.WriteLine(post.Children.First().Title);
            var joined = await DbContext.GroupMembers
                                .Where(u => u.UserId == currentuser.Id)
                                .Select(u => u.GroupId).ToListAsync();

            PostViewModel model = new PostViewModel {Seed = post.PostId, post = post, Joined = joined, Children = post.Children};
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, CancellationToken requestAborted, IFormFile file, string id)
        {
            
            Console.WriteLine("ID: " + id);
            System.Diagnostics.Debug.WriteLine("ID " + id);
            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid && user != null)
            {
                
                post.User = user;
                if((file != null) && (file.Length > 0))
                {               
                    post.Image = await Storage.GetUri("postimages", Guid.NewGuid().ToString(), file);
                }
                post.GroupGroupId = id;
                DbContext.Posts.Add(post);
                await DbContext.SaveChangesAsync(requestAborted);

                var following = DbContext.FollowRelations
                        .Where(u => u.FollowingId == user.Id)
                        .Include(u => u.Follower)
                        .Select(u => u.Follower.Id)
                        .ToList();
                var usernames = DbContext.Users
                        .Where(u => following.Contains(u.Id))
                        .Select(u => u.UserName).ToList();

                _feedHub.Clients.Users(usernames).feed(post, user);
                //_feedHub.Clients.All.feed(postdata);

                Cache.Remove("latestPost");
                //return RedirectToAction("Index");
                return RedirectToAction("Index");
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
            var VoteRelations = new VoteRelation() {PostId = postId, UserId = Context.User.GetUserId()};
            DbContext.VoteRelations.Add(VoteRelations);
            await DbContext.SaveChangesAsync();
            
            
            return Json(new {responseText = "Success!" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Post post, CancellationToken requestAborted)
        {
            var postData = await DbContext.Posts.SingleAsync(postTable => postTable.PostId == post.PostId);
            postData.Title = post.Title;
            postData.Title = post.Text;
            await DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Post post, CancellationToken requestAborted)
        {
            var postData = await DbContext.Posts.SingleAsync(postTable => postTable.PostId == post.PostId);
            DbContext.Remove(postData);
            await DbContext.SaveChangesAsync(requestAborted);
            return View();
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
