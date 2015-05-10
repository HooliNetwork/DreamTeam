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

            services.AddMvc();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task BanUserTest()
        {
            // Arrange
            var controller = new GroupController();
            var dbContext = _serviceProvider.GetRequiredService<HooliContext>();
            var user = new ApplicationUser() { UserName = "TestUser" };
            List<ApplicationUser> memberList = new List<ApplicationUser>();
            memberList.Add(user);
            var group = new Group() { GroupId = 1, Members = memberList, BannedUsers = null };
            dbContext.Add(group);

            // Act
            var result = await controller.BanUser(1, user, CancellationToken.None);

            // Assert
            Assert.True(group.BannedUsers.Contains(user));
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
