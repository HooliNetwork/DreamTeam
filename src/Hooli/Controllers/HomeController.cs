﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Hooli.Models;
using Microsoft.Framework.Caching.Memory;
using System.Threading;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Authorization;
using System.Dynamic;
using Hooli.ViewModels;
using Hooli.Components;
using Hooli.Services;

namespace Hooli.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [FromServices]
        public HooliContext DbContext { get; set; }

        [FromServices]
        public UserService UserService { get; set; }

        public FeedComponent feedComponent { get; set; }
        public HomeController(FeedComponent _feedComponent)
        {
            feedComponent = _feedComponent;
        }

        [FromServices]
        public IMemoryCache Cache { get; set; }

        //
        // GET: /Home/
        public async Task<IActionResult> Index(string sortOrder)
        {
            var userId = Context.User.GetUserId();
            var people = await UserService.GetFollowedPeople(6, userId);
            var groups = await UserService.GetFollowedGroups(6, userId);
            var interestingGroups = false;
            var interestingPeople = false;
            if (groups.Count() < 6)
            {
                groups = await UserService.GetInterestingGroups(6, userId);
                interestingGroups = true;
            }
            if (people.Count() < 6)
            {
                people = await UserService.GetInterestingPeople(6, userId);
                interestingPeople = true;
            }
            return View(new HomeViewModel {
                Groups = groups,
                People = people,
                InterestingPeople = interestingPeople,
                InterestingGroups = interestingGroups}
            );
            
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

        [HttpPost]
        public async Task<IActionResult> Search(string searchString)
        {
            dynamic model = new ExpandoObject();

            if (!String.IsNullOrEmpty(searchString))
            {

                var userId = Context.User.GetUserId();

                model.Users = await DbContext.Users.Where(s => s.LastName
                                            .Contains(searchString)
                                            || s.FirstName.Contains(searchString))
                                            .ToListAsync();

                model.Groups = await DbContext.Groups.Where(g => g.GroupName
                                            .Contains(searchString))
                                            .ToListAsync();

                model.Following = await UserService.GetFollowedPeopleIds(userId);
                model.Joined = await UserService.GetFollowedGroupsIds(userId);

                return View(model);
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult Sort(bool latestPosts, bool group, string groupId)
        {
            // Wrap the component to be able to post to it.
            dynamic model = new ExpandoObject();
            model.latestPost = latestPosts;
            model.group = group;
            model.groupId = groupId;
            return PartialView(model);
        }

    }
}
