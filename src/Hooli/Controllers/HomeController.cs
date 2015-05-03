using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Hooli.Models;
using Microsoft.Framework.Caching.Memory;

namespace Hooli.Controllers
{
    public class HomeController : Controller
    {
        [FromServices]
        public HooliContext DbContext { get; set; }

        [FromServices]
        public IMemoryCache Cache { get; set; }

        public IActionResult Index()
        {
            return View();
        }
        //
        // GET: /Home/
        public async Task<IActionResult> Index2()
        {
            // Get most popular Posts
            var Posts = await Cache.GetOrSet("top", async context =>
            {
                //Refresh it every 10 minutes. Let this be the last item to be removed by cache if cache GC kicks in.
                context.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                context.SetPriority(CachePreservationPriority.High);
                return await GetTopPost(10);
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
                .ToListAsync();
        }
    }
}
