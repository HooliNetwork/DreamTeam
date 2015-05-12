using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Hooli.Models;
using System.Threading;
using Microsoft.AspNet.Authorization;



// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Hooli.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class GroupController : Controller
    {
        [FromServices]
        public HooliContext DbContext { get; set; }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGroup(Group group, CancellationToken requestAborted)
        {
            System.Diagnostics.Debug.WriteLine("Inside CreateGroup function Trol");
            System.Diagnostics.Debug.WriteLine("Model state" + ModelState);
            if (ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("Inside if sentence");
                DbContext.Groups.Add(group);
                System.Diagnostics.Debug.WriteLine("Inside if sentence 2");
                await DbContext.SaveChangesAsync(requestAborted);
                System.Diagnostics.Debug.WriteLine("Inside if sentence 3");


                var groupData = new Group
                {
                    GroupName = group.GroupName,
                    Description = group.Description,
                    Private = group.Private,
                    //Members = group.Members
                  };

                System.Diagnostics.Debug.WriteLine("Inside if sentence 4");

            System.Diagnostics.Debug.WriteLine(group.DateCreated);
       
            System.Diagnostics.Debug.WriteLine("Inside if sentence 5");


            return RedirectToAction("Index");
            }

                System.Diagnostics.Debug.WriteLine("Leaving CreateGroup function");
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
        // GET: /Account/SingleGroup
        [HttpGet("SingleGroup/{id}")]
        public IActionResult SingleGroup(int id)
        {
            
            return View();
        }


    }
}
