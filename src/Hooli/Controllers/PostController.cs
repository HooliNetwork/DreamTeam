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
            dynamic model = new ExpandoObject();
            System.Diagnostics.Debug.WriteLine(id);
            model.post = await DbContext.Posts
                 .Include(u => u.User)
                 .Include(g => g.Group)
                 .SingleAsync(p => p.PostId == id);
            model.Joined = await DbContext.GroupMembers
                                .Where(u => u.UserId == currentuser.Id)
                                .Select(u => u.GroupId).ToListAsync();
            return View(model);
        }


        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, CancellationToken requestAborted, IFormFile file, string id)
        {

            Console.WriteLine("ID: " + id);
            System.Diagnostics.Debug.WriteLine("ID " + id);
            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid && user != null)
            {
                Console.WriteLine("pc1");
                post.User = user;
                if((file != null) && (file.Length > 0))
                {               
                    post.Image = await Storage.GetUri("postimages", Guid.NewGuid().ToString(), file);
                }
                Console.WriteLine("pc2");
                post.GroupGroupId = id;
                DbContext.Posts.Add(post);
                await DbContext.SaveChangesAsync(requestAborted);
                Console.WriteLine("pc3");
                var postdata = new PostData
                {
                    Title = post.Title,
                    // We might want link to the post
                    //Url = Url.Action("Details", "Post", new { id = post.PostId })
                    Text = post.Text
                };
                Console.WriteLine("pc4");
                var following = DbContext.FollowRelations
                        .Where(u => u.FollowingId == user.Id)
                        .Include(u => u.Follower)
                        .Select(u => u.FollowerId)
                        .ToList();
                Console.WriteLine("pc5");
                var usernames = DbContext.Users
                        .Where(u => following.Contains(u.Id))
                        .Select(u => u.UserName).ToList();
                Console.WriteLine("pc6");

                foreach (object o in following)
                {
                    Console.WriteLine(o);
                }
                Console.WriteLine(Context.User.Identity.Name);

                _feedHub.Clients.Users(usernames).feed(postdata);
                //_feedHub.Clients.All.feed(postdata);
                Console.WriteLine("pc7");

                Cache.Remove("latestPost");
                Console.WriteLine("pc8");

                return Redirect("Post/Index/" + post.PostId);
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


    }
}
