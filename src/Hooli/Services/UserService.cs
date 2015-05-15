using Hooli.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNet.Authorization;
using System.Security.Claims;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;

namespace Hooli.Services
{
    public class UserService
    {
        public HooliContext DbContext;

        public UserService(HooliContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<string> GetProfilePictureUrl(string userId)
        {
            var user = await GetUser(userId);
            if (user.ProfilePicture != null) {
                return user.ProfilePicture;              
            } else return "https://hoolidata.blob.core.windows.net/profilepictures/86f53944-3e52-4469-8bfe-df0b57745469";
  
        }
        public async Task<ApplicationUser> GetUser(string userId)
        {
            return await DbContext.Users.SingleAsync(u => u.Id == userId);
        }

        public async Task<string> GetLastname(string userId)
        {
            var user = await GetUser(userId);
            return user.LastName;
        }

        public async Task<string> GetFirstname(string userId)
        {
            var user = await GetUser(userId);
            return user.FirstName;
        }

        public async Task<DateTime> GetDateOfBirth(string userId)
        {
            var user = await GetUser(userId);
            return user.DateOfBirth;
        }

        public async Task<string> GetUserName(string userId)
        {
            var user = await GetUser(userId);
            return user.UserName;
        }

        public async Task<string> GetAge(string userId)
        {
            var user = await GetUser(userId);
            if (user != null)
            {
                DateTime today = DateTime.Today;
                int age = today.Year - user.DateOfBirth.Year;
                if (user.DateOfBirth > today.AddYears(-age)) age--;
                return age.ToString();
            }
            return "0";
        }
        // Get a list of groups that the user is not following
        public async Task<List<Group>> GetFollowedGroups(int count, string userId)
        {
            var following = await GetFollowedGroupsIds(userId);

            return await DbContext.Groups
                                  .Where(u => following.Contains(u.GroupId))
                                  .Take(count)
                                  .ToListAsync();
        }

        public async Task<List<Group>> GetInterestingGroups(int count, string userId)
        {
            var following = await GetFollowedGroupsIds(userId);

            return await DbContext.Groups
                                  .Where(u => !following.Contains(u.GroupId))
                                  .Take(count)
                                  .ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetFollowedPeople(int count, string userId)
        {
            var following = await GetFollowedPeopleIds(userId);

            return await DbContext.Users
                                  .Where(u => following.Contains(u.Id))
                                  .Take(count)
                                  .ToListAsync();
        }

        // Get a list of people that the user is not following
        public async Task<List<ApplicationUser>> GetInterestingPeople(int count, string userId)
        {
            var following = await GetFollowedPeopleIds(userId);

            return await DbContext.Users
                                  .Where(u => !following.Contains(u.Id))
                                  .Take(count)
                                  .ToListAsync();
        }

        // Get the Id's of people that the user has followed
        public async Task<List<string>> GetFollowedPeopleIds(string userId)
        {
            return await DbContext.FollowRelations
                                  .Where(u => u.FollowerId == userId)
                                  .Select(u => u.FollowingId)
                                  .ToListAsync();
        }

        // Get the Id's of people that are following the user
        public async Task<List<string>> GetFollowingPeopleIds(string userId)
        {
            return await DbContext.FollowRelations
                                  .Where(u => u.FollowingId == userId)
                                  .Select(u => u.FollowerId)
                                  .ToListAsync();
        }

        // Get the Id's of people that the user has followed
        public async Task<List<string>> GetFollowedGroupsIds(string userId)
        {
            return await DbContext.GroupMembers
                                  .Where(u => u.UserId == userId)
                                  .Select(u => u.GroupId)
                                  .ToListAsync();
        }

        // Get the Id's of people that are following the user
        public async Task<List<string>> GetFollowingGroupsIds(string userId)
        {
            return await DbContext.GroupMembers
                                  .Where(u => u.UserId == userId)
                                  .Select(u => u.GroupId)
                                  .ToListAsync();
        }
    }
}
