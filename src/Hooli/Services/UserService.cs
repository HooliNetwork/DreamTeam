using Hooli.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;

namespace Hooli.Services
{
    public class UserService
    {

        public HooliContext DbContext { get; set; }
        public UserService(HooliContext dbContext)
        {
            DbContext = dbContext;
        }
        public async Task<string> GetProfilePictureUrl(string userId)
        {
            Console.WriteLine(userId);
            var user = await DbContext.Users.SingleAsync(u => u.Id == userId);
            Console.WriteLine(user.ProfilePicture);
            return user.ProfilePicture;
        }
    }
}
