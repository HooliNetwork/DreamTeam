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
            var userId = "5";
            var groupID = "5";
            var user = new ApplicationUser() { UserName = "Test", Id = userId};
            var group = new Group() {GroupName = "Cool People" , GroupId = groupID };
            var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var userManagerResult = await userManager.CreateAsync(user);
            var dbContext = _serviceProvider.GetRequiredService<HooliContext>();
            var groupMemberUser = new GroupMember() { GroupId = "5", UserId = "5", banned = false, Group = group, Member = user};
            dbContext.Add(groupMemberUser);
            dbContext.SaveChanges();

              var controller = new GroupController()
            {
                DbContext = dbContext,
            };

            // Act BanUser
            await controller.BanUser("5", "5", CancellationToken.None);

            // Assert BanUser
            Assert.True(groupMemberUser.banned == true);

            // Act UnBanUser
            await controller.UnBanUser("5", "5", CancellationToken.None);

            // Assert UnBanUser
            Assert.True(groupMemberUser.banned == false);
        }
    }

}
