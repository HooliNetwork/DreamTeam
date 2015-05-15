using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Hooli.Models;
using System.Threading;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.AspNet.Http;
using Hooli.CloudStorage;
using System.Dynamic;


namespace Hooli.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        [FromServices]
        public HooliContext DbContext { get; set; }

        [FromServices]
        public Cloud storage { get; set; }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGroup(Group group, CancellationToken requestAborted, IFormFile file)
        {

            var userId = Context.User.GetUserId();
            if (ModelState.IsValid)     
            {

                if ((file != null) && (file.Length > 0))
                {
                    group.Image = await storage.GetUri("postimages", Guid.NewGuid().ToString(), file);
                }

                DbContext.Groups.Add(group);

                var groupmember = new GroupMember() { GroupId = group.GroupId, UserId=userId, banned = false };

                DbContext.GroupMembers.Add(groupmember);


                await DbContext.SaveChangesAsync(requestAborted);




            System.Diagnostics.Debug.WriteLine(group.DateCreated);
      
            return RedirectToAction("Index");
              
            }

            return View(group);
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGroup(Group group, CancellationToken requestAborted)
        {
            if (ModelState.IsValid)
            {
                var groupData = DbContext.Groups.Single(groupTable => groupTable.GroupId == group.GroupId);

                groupData.GroupName = group.GroupName;
                groupData.Description = group.Description;
                groupData.Private = group.Private;
                groupData.Members = group.Members;
                groupData.Image = group.Image;
                await DbContext.SaveChangesAsync(requestAborted);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPostToGroup(string groupId, Post post, CancellationToken requestAborted)
        {
            var group = DbContext.Groups.Single(groupTable => groupTable.GroupId == groupId);
            group.Posts.Add(post);
            await DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BanUser(string groupId, string userId, CancellationToken requestAborted)
        {
            var ban = await DbContext.GroupMembers.SingleAsync(u => (u.GroupId == groupId) && (u.UserId == userId));
            ban.banned = true;
            await DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnBanUser(string groupId, string userId, CancellationToken requestAborted)
        {
            var unban = await DbContext.GroupMembers.SingleAsync(u => (u.GroupId == groupId) && (u.UserId == userId));
            unban.banned = false;
            await DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        // GET: /Group/SingleGroup
        [HttpGet]
        public async Task<IActionResult> SingleGroup(string id)
        {
            dynamic model = new ExpandoObject();
            var userId = Context.User.GetUserId();
            model.group = await DbContext.Groups
                    .Where(a => a.GroupId == id)
                    .FirstOrDefaultAsync();
            model.Joined = await DbContext.GroupMembers
                                .Where(u => u.UserId == userId)
                                .Select(u => u.GroupId).ToListAsync();

            if (model.group == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        private async Task<List<Group>> GetGroups(IEnumerable<string> group)
        {
            var groups = await DbContext.Groups
                .Where(g => group.Contains(g.GroupId))
                .ToListAsync();
            return groups;
        }
    }
}
