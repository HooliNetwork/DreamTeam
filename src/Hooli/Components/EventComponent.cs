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
    [ViewComponent(Name = "Event")]
    public class EventComponent : ViewComponent
    {
        public EventComponent(UserManager<ApplicationUser> userManager)
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
            var events = await Cache.GetOrSet("event", async context =>
            {
                context.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                return await GetUserEvents();
            });
            return View(events);
        }

        private async Task<Event> GetUserEvents()
        {
            var user = await GetCurrentUserAsync();
            var events = user.Events.Select(a => a.EventId);
            var userEvents = await DbContext.Events.OrderByDescending(a => a.EventName).Where(a => (events.Contains(a.EventId))).FirstOrDefaultAsync();
            return userEvents;
        }
    

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.GetUserId());
        }
    }
}
