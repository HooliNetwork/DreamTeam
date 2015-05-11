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
    public class EventController : Controller
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
        public async Task<IActionResult> CreateEvent(Event newEvent, CancellationToken requestAborted)
        {
            var eventData = new Event
            {
                EventName = newEvent.EventName,
                StartTime = newEvent.StartTime,
                EndTime = newEvent.EndTime,
                Private = newEvent.Private,
                Description = newEvent.Description,
                Location = newEvent.Location,
                Image = newEvent.Image,
                DateCreated = newEvent.DateCreated,              
            };
            DbContext.Events.Add(eventData);
            await DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(Event newEvent, CancellationToken requestAborted)
        {
            var eventData = DbContext.Events.Single(eventTable => eventTable.EventId == newEvent.EventId);

            eventData.EventName = newEvent.EventName;
            eventData.Description = newEvent.Description;
            eventData.StartTime = newEvent.StartTime;
            eventData.EndTime = newEvent.EndTime;
            eventData.Location = newEvent.Location;
            eventData.Private = newEvent.Private;
            eventData.Image = newEvent.Image;

            await DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserToAttending(int eventId, ApplicationUser user, CancellationToken requestAborted)
        {
            var eventData = DbContext.Events.Single(eventTable => eventTable.EventId == eventId);
            eventData.AttendingUsers.Add(user);
            await DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserToInvited(int eventId, ApplicationUser user, CancellationToken requestAborted)
        {
            var eventData = DbContext.Events.Single(eventTable => eventTable.EventId == eventId);
            eventData.InvitedUsers.Add(user);
            await DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPostToEvent(int eventId, Post post, CancellationToken requestAborted)
        {
            var eventData = DbContext.Events.Single(eventTable => eventTable.EventId == eventId);
            eventData.Posts.Add(post);
            await DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

    }
}
