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



// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Hooli.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        [FromServices]
        public HooliContext DbContext { get; set; }

        [FromServices]
        public Cloud storage { get; set; }

        [FromServices]
        public UserManager<ApplicationUser> UserManager { get; set; }

        // GET: /<controller>/

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var groups = DbContext.GroupMembers
                    .Where(u => u.UserId == user.Id)
                    .Select(u => u.GroupId).ToList();
            //  return View( await GetGroups(groups));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGroup(Group group, CancellationToken requestAborted, IFormFile file)
        {
            var user = await GetCurrentUserAsync();
            if (ModelState.IsValid && user != null)     
            {

                if ((file != null) && (file.Length > 0))
                {
                    group.Image = await storage.GetUri("postimages", Guid.NewGuid().ToString(), file);
                }

                DbContext.Groups.Add(group);

                var groupmember = new GroupMember() { GroupId = group.GroupId, UserId=user.Id, banned = false };

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
                DbContext.Update(group);
                await DbContext.SaveChangesAsync(requestAborted);
            }

            //var groupData = DbContext.Groups.Single(groupTable => groupTable.GroupId == group.GroupId);

            //groupData.GroupName = group.GroupName;
            //groupData.Description = group.Description;
            //groupData.Private = group.Private;
            //groupData.DateCreated = group.DateCreated;
            //groupData.Members = group.Members;
            //groupData.GroupPicture = group.GroupPicture;

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
        public async Task<IActionResult> BanUser(string groupId, ApplicationUser user, CancellationToken requestAborted)
        {
            var ban = await DbContext.GroupMembers.SingleAsync(u => (u.GroupId == groupId) && (u.UserId == user.Id));
            ban.banned = true;
            await DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnBanUser(string groupId, ApplicationUser user, CancellationToken requestAborted)
        {
            var unban = await DbContext.GroupMembers.SingleAsync(u => (u.GroupId == groupId) && (u.UserId == user.Id));
            unban.banned = false;
            await DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        //
        // GET: /Group/SingleGroup
        [HttpGet]
        public IActionResult SingleGroup(string id)
        {
            
            return View();
        }


        private async Task<List<Group>> GetGroups(IEnumerable<string> group)
        {
            System.Diagnostics.Debug.WriteLine("Inside the getgroup function");
            var groups = await DbContext.Groups
                .Where(g => group.Contains(g.GroupId))
                .ToListAsync();
            return groups;
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.GetUserId());
        }

    }
}
