using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Core;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Session;
using Microsoft.Framework.Caching.Distributed;
using Microsoft.Framework.Caching.Memory;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging.Testing;
using Hooli.Models;
using Hooli.ViewModels;
using Xunit;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity;

namespace Hooli.Controllers
{
    public class GroupControllerTest
    {
        private readonly IServiceProvider _serviceProvider;

        public GroupControllerTest()
        {
            var services = new ServiceCollection();
            services.AddEntityFramework()
                    .AddInMemoryStore()
                    .AddDbContext<HooliContext>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<HooliContext>();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task BanUserTest()
        {
            // Arrange
            var userId = "1";
            var groupId = "Bicycle";
            var user = new ApplicationUser() { UserName = "Test", Id = userId};
            var group = new Group() {GroupName = "Bicycle", GroupId = groupId };
            var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var userManagerResult = await userManager.CreateAsync(user);
            var dbContext = _serviceProvider.GetRequiredService<HooliContext>();
            var groupMemberUser = new GroupMember() { GroupId = groupId, UserId = userId, banned = false, Group = group, Member = user};
            dbContext.Add(groupMemberUser);
            dbContext.SaveChanges();

              var controller = new GroupController()
            {
                DbContext = dbContext,
            };

            // Act BanUser
            await controller.BanUser(groupId, userId, CancellationToken.None);

            // Assert BanUser
            Assert.True(groupMemberUser.banned == true);

            // Act UnBanUser
            await controller.UnBanUser(groupId, userId, CancellationToken.None);

            // Assert UnBanUser
            Assert.True(groupMemberUser.banned == false);
        }

        [Fact]
        public async Task AddPostToGroupTest()
        {
            // Arrange
            // Everything with userId should propably be erased
            //var userId = "1";
            //var user = new ApplicationUser() { UserName = "Test", Id = userId };
            //var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            //var userManagerResult = await userManager.CreateAsync(user);
            //var post = new Post() { PostId = postId, UserId = userId };


            var postId = 1;
            var groupId = "1";
            var post = new Post() { PostId = postId };
            var group = new Group() { GroupId = groupId, GroupName = groupId };
            var dbContext = _serviceProvider.GetRequiredService<HooliContext>();

            dbContext.Add(group);
            dbContext.Add(post);
            dbContext.SaveChanges();

            var controller = new GroupController()
            {
                DbContext = dbContext,
            };

            // Act
            await controller.AddPostToGroup(groupId, post, CancellationToken.None);

            // Assert
            Assert.True(group.Posts.Count == 1);
        }

        [Fact]
        public async Task EditGroupTest()
        {
            // Arrange
            var groupId = "1";
            var changedGroupId = "2";
            var group = new Group() { GroupId = groupId, GroupName = groupId };
            var dbContext = _serviceProvider.GetRequiredService<HooliContext>();
            var changedGroup = new Group() { GroupId = changedGroupId, GroupName = changedGroupId };

            dbContext.Add(group);
            dbContext.SaveChanges();

            var controller = new GroupController()
            {
                DbContext = dbContext,
            };

            // Act
            await controller.EditGroup(changedGroup, CancellationToken.None);

            // Assert
            Assert.True(group.GroupName == changedGroup.GroupName);
        }

        [Fact]
        public async Task CreateGroupTest()
        {
            // Arrange
            var groupId = "1";
            var group = new Group() { GroupId = groupId, GroupName = groupId };
            var dbContext = _serviceProvider.GetRequiredService<HooliContext>();

            var controller = new GroupController()
            {
                DbContext = dbContext,
            };
            // Act
            await controller.CreateGroup(group, CancellationToken.None);

            // Assert
            Assert.False(controller.DbContext.Groups.Single(u => u.GroupId == groupId).Equals(null));
        }
    }

}
