using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;
using Hooli.Models;
using Xunit;
using Microsoft.AspNet.Identity.EntityFramework;
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
            var user = new ApplicationUser() { UserName = "Test", Id = userId };
            var group = new Group() { GroupName = "Bicycle", GroupId = groupId };
            var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var userManagerResult = await userManager.CreateAsync(user);
            var dbContext = _serviceProvider.GetRequiredService<HooliContext>();
            var groupMemberUser = new GroupMember() { GroupId = groupId, UserId = userId, banned = false, Group = group, Member = user };
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
        public async Task EditGroupTest()
        {
            // Arrange
            var groupId = "1";
            var group = new Group() { GroupId = groupId, GroupName = "Name1" };
            var dbContext = _serviceProvider.GetRequiredService<HooliContext>();
            var changedGroup = new Group() { GroupId = groupId, GroupName = "Name2" };

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
            Assert.True(group.Description == changedGroup.Description);
            Assert.True(group.Image == changedGroup.Image);
            Assert.True(group.Private == changedGroup.Private);
            Assert.True(group.Members == changedGroup.Members);
            Assert.True(group.Posts == changedGroup.Posts);
        }
    }

}
