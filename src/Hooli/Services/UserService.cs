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
            if (user.ProfilePicture != null) {
                return user.ProfilePicture;              
            } else return "https://hoolidata.blob.core.windows.net/profilepictures/86f53944-3e52-4469-8bfe-df0b57745469";
  
        }
        
        public async Task<string> GetAge(string userId)
        {
            var user = await DbContext.Users.SingleAsync(u => u.Id == userId);
            if (user != null)
            {
                DateTime today = DateTime.Today;
                int age = today.Year - user.DateOfBirth.Year;
                if (user.DateOfBirth > today.AddYears(-age)) age--;
                return age.ToString();
            }
            return "0";
        }

    }
}
