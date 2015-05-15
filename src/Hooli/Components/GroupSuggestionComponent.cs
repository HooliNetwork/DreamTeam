using Hooli.Services;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hooli.ViewModels;

namespace Hooli.Components
{
    [ViewComponent(Name = "GroupSuggestion")]
    public class GroupSuggestionComponent : ViewComponent
    {

        [Activate]
        public UserService UserService { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(bool showInteresting)
        {
            var userId = Context.User.GetUserId();
            var groups = await UserService.GetFollowedGroups(6, userId);

            if (showInteresting || (groups.Count() < 6))
            {
                groups = await UserService.GetInterestingGroups(6, userId);
            }

            return View(new SuggestionGroupViewModel
            {
                Groups = groups,
                ShowInteresting = showInteresting
            });
        }

    }
}
