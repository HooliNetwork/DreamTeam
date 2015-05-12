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
        public Cloud storage { get; set; }

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

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, CancellationToken requestAborted, IFormFile file)
        {
            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid && user != null)
            {
                
                post.User = user;
                if((file != null) && (file.Length > 0))
                {               
                    post.Image = await storage.GetUri("postimages", Guid.NewGuid().ToString(), file);
                }
                
                DbContext.Posts.Add(post);
                await DbContext.SaveChangesAsync(requestAborted);
                
                var postdata = new PostData
                {
                    Title = post.Title,
                    // We might want link to the post
                    //Url = Url.Action("Details", "Post", new { id = post.PostId })
                    Text = post.Text
                };
                var following = DbContext.FollowRelations
                        .Where(u => u.FollowingId == user.Id)
                        .Select(u => u.FollowerId)
                        .ToList();
                foreach (object o in following)
                {
                    Console.WriteLine(o);
                }
                Console.WriteLine(Context.User.Identity.Name);
                
                _feedHub.Clients.User(Context.User.Identity.Name).feed(postdata);
                //_feedHub.Clients.All.feed(postdata);
                
                Cache.Remove("latestPost");
                return RedirectToAction("Index");
            }
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upvote(Post post, CancellationToken requestAborted)
        {
            var postData = await DbContext.Posts.SingleAsync(postTable => postTable.PostId == post.PostId);
            postData.Points++;
            await DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Downvote(Post post, CancellationToken requestAborted)
        {
            var postData = await DbContext.Posts.SingleAsync(postTable => postTable.PostId == post.PostId);
            postData.Points--;
            await DbContext.SaveChangesAsync(requestAborted);
            return View();
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
