using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Hooli.Models;
using Microsoft.Framework.Caching.Memory;
using System.Threading;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace Hooli.Controllers
{
    public class HomeController : Controller
    {
        [FromServices]
        public HooliContext DbContext { get; set; }
        public HomeController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }
        public UserManager<ApplicationUser> UserManager { get; private set; }

        [FromServices]
        public IMemoryCache Cache { get; set; }
        //
        // GET: /Home/
        public async Task<IActionResult> Index()
        {
            // Get most popular Posts
            var Posts = await Cache.GetOrSet("top", async context =>
            {
                //Refresh it every 10 minutes. Let this be the last item to be removed by cache if cache GC kicks in.
                context.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                context.SetPriority(CachePreservationPriority.High);
                return await GetTopPost(4);
            });
            return View(Posts);
        }

        //Can be removed and handled when HandleError filter is implemented
        //https://github.com/aspnet/Mvc/issues/623
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        public IActionResult StatusCodePage()
        {
            return View("~/Views/Shared/StatusCodePage.cshtml");
        }

        

        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private async Task<List<Post>> GetTopPost(int count)
        {
            // Group the order details by Post and return
            // the Posts with the highest count of Upvotes

            // TODO [EF] We don't query related data as yet, so the OrderByDescending isn't doing anything
            return await DbContext.Post
                .OrderByDescending(a => a.UpVotes)
                .Take(count)
                .Include(u => u.User)
                .ToListAsync();
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
                DbContext.Post.Add(post);
                await DbContext.SaveChangesAsync(requestAborted);

                //  Live feed here
                //
                //var postdata = new PostData
                //{
                //    Title = post.Title,
                //    Url = Url.Action("Details", "Store", new { id = album.AlbumId })
                //};
                //_announcementHub.Clients.All.announcement(postdata);
                //Cache.Remove("latestAlbum");

                return RedirectToAction("Index");
            }
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
