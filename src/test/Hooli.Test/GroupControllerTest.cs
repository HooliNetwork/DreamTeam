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

            services.AddMvc();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task BanUserTest()
        {
            // Arrange
            var dbContext = _serviceProvider.GetRequiredService<HooliContext>();
            var user = new ApplicationUser() { UserName = "TestUser", Id = "1"};
            var group = new Group() { GroupId = "1", GroupName = "Cool People"};
            var groupMemberUser = new GroupMember() { GroupId = "1", UserId = "1", banned = false, Group = group, Member = user};
            group.Members.Add(groupMemberUser);
            dbContext.Add(user);
            // dbContext.Add(groupMemberUser);
            dbContext.Add(group);
            dbContext.SaveChanges();
            var controller = new GroupController()
            {
                DbContext = dbContext,
            };

            // Act
            var result = await controller.BanUser(group.GroupId, user, CancellationToken.None);

            // Assert
            //Assert.True(true);
            Assert.True(groupMemberUser.banned );
        }

        [Fact]
        public async Task UnBanUserTest()
        {
            // Arrange
            var dbContext = _serviceProvider.GetRequiredService<HooliContext>();
            var user = new ApplicationUser() { UserName = "TestUser", Id = "1" };
            var group = new Group() { GroupId = "1", GroupName = "Cool People" };
            var groupMemberUser = new GroupMember() { GroupId = "1", UserId = "1", banned = false, Group = group, Member = user };
            group.Members.Add(groupMemberUser);
            dbContext.Add(user);
            dbContext.Add(groupMemberUser);
            dbContext.Add(group);
            dbContext.SaveChanges();
            var controller = new GroupController()
            {
                DbContext = dbContext,
            };

            // Act
            var result = await controller.BanUser(group.GroupId, user, CancellationToken.None);

            // Assert
            Assert.True(!groupMemberUser.banned);
        }

        //private static ISession CreateTestSession()
        //{

        //    return new DistributedSession(
        //        new LocalCache(new MemoryCache(new MemoryCacheOptions())),
        //        "sessionId_A",
        //        idleTimeout: TimeSpan.MaxValue,
        //        tryEstablishSession: () => true,
        //        loggerFactory: new NullLoggerFactory(),
        //        isNewSessionKey: true);
        //}


    }

}
