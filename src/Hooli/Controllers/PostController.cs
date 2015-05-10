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

        // POST: /StoreManager/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, CancellationToken requestAborted)
        {
            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid && user != null)
            {
                post.User = user;
                DbContext.Posts.Add(post);
                await DbContext.SaveChangesAsync(requestAborted);

                var postdata = new PostData
                {
                    Title = post.Title,
                    // We might want link to the post
                    //Url = Url.Action("Details", "Post", new { id = post.PostId })
                    Text = post.Text
                };
                _feedHub.Clients.Users(user.Following.Select(c => c.FollowerId).ToList()).feed(postdata);
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
            var postData = DbContext.Posts.Single(postTable => postTable.PostId == post.PostId);
            postData.Points++;
            await DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Downvote(Post post, CancellationToken requestAborted)
        {
            var postData = DbContext.Posts.Single(postTable => postTable.PostId == post.PostId);
            postData.Points--;
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
