using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Hooli.Models;
using System.Threading;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Hooli.Controllers
{
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
        public IActionResult CreateEvent(Event newEvent, CancellationToken requestAborted)
        {
            var eventData = new Event
            {
                EventName = newEvent.EventName,
                StartTime = newEvent.StartTime,
                EndTime = newEvent.EndTime,
                Private = newEvent.Private,
                Description = newEvent.Description,
                Location = newEvent.Location,
                ImgUrl = newEvent.ImgUrl,
                DateCreated = newEvent.DateCreated,              
            };
            DbContext.Event.Add(eventData);
            DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEvent(Event newEvent, CancellationToken requestAborted)
        {
            var eventData = DbContext.Event.Single(eventTable => eventTable.EventId == newEvent.EventId);

            eventData.EventName = newEvent.EventName;
            eventData.Description = newEvent.Description;
            eventData.StartTime = newEvent.StartTime;
            eventData.EndTime = newEvent.EndTime;
            eventData.Location = newEvent.Location;
            eventData.Private = newEvent.Private;
            eventData.ImgUrl = newEvent.ImgUrl;

            DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUserToAttending(int eventId, ApplicationUser user, CancellationToken requestAborted)
        {
            var eventData = DbContext.Event.Single(eventTable => eventTable.EventId == eventId);
            eventData.AttendingUsers.Add(user);
            DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUserToInvited(int eventId, ApplicationUser user, CancellationToken requestAborted)
        {
            var eventData = DbContext.Event.Single(eventTable => eventTable.EventId == eventId);
            eventData.InvitedUsers.Add(user);
            DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPostToEvent(int eventId, Post post, CancellationToken requestAborted)
        {
            var eventData = DbContext.Event.Single(eventTable => eventTable.EventId == eventId);
            eventData.Posts.Add(post);
            DbContext.SaveChangesAsync(requestAborted);
            return View();
        }

    }
}
