using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Caching.Memory;
using Hooli.Models;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace Hooli.Components
{
    [ViewComponent(Name = "People")]
    public class PeopleComponent : ViewComponent
    {
        public PeopleComponent(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }
        public UserManager<ApplicationUser> UserManager { get; private set; }

        [Activate]
        public HooliContext DbContext
        {
            get;
            set;
        }

        [Activate]
        public IMemoryCache Cache
        {
            get;
            set;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            System.Diagnostics.Debug.WriteLine("The Invoke");
            //var group = await Cache.GetOrSet("group", async context =>
            //{
            //    context.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            //    return await GetUserGroups();
            //});
            //return View(group);
            var user = await GetCurrentUserAsync();
            var people = DbContext.FollowRelations
                           .Where(u => u.FollowerId == user.Id)
                           .Select(u => u.FollowingId).ToList();
            return View(await GetPeople(people));
        }

        //private async Task<Group> GetUserGroups()
        //{
        //    var user = await GetCurrentUserAsync();
        //    var groups = user.GroupsMember.Select(a => a.GroupId);
        //    var userGroups = await DbContext.Groups.OrderByDescending(a => a.GroupName).Where(a => (groups.Contains(a.GroupId))).FirstOrDefaultAsync();
        //    return userGroups;
        //}

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.GetUserId());
        }

        private async Task<List<ApplicationUser>> GetPeople(IEnumerable<string> people)
        {
            System.Diagnostics.Debug.WriteLine("Inside the getgroup function");
            var peoples = await DbContext.Users
                .Where(p => people.Contains(p.Id))
                .ToListAsync();
            return peoples;
        }

    }


}
