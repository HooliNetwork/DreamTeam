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
using Hooli.Services;

namespace Hooli.Components
{
    [ViewComponent(Name = "Group")]
    public class GroupComponent : ViewComponent
    {

        [Activate]
        public UserService UserService { get; set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View( await UserService.GetFollowedGroups(20, Context.User.GetUserId()));
        }

    }


}
