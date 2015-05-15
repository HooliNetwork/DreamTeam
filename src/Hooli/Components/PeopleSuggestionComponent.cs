using Hooli.Services;
using Hooli.ViewModels;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hooli.Components
{
    [ViewComponent(Name = "PeopleSuggestion")]
    public class PeopleSuggestionComponent : ViewComponent
    {

        [Activate]
        public UserService UserService { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(bool showInteresting)
        {

            var userId = Context.User.GetUserId();
            var people = await UserService.GetFollowedPeople(6, userId);

            if (showInteresting || (people.Count() < 6))
            {
                people = await UserService.GetInterestingPeople(6, userId);
                showInteresting = true;
            }
            return View(new SuggestionGroupViewModel
            {
                People = people,
                ShowInteresting = showInteresting
            });
        }

    }
}
