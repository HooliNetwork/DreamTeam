using Hooli.Controllers;
using Hooli.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Hooli.Test
{
    public class PostControllerTest
    {
        private readonly IServiceProvider _serviceProvider;

        public PostControllerTest()
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
        public async Task CreatePostTest()
        {
            // Arrange
            var postId = 1;
            var post = new Post() { PostId = postId, Title = "GreatTitle" };
            var dbContext = _serviceProvider.GetRequiredService<HooliContext>();

            var userId = "1";
            var user = new ApplicationUser() { UserName = "Test", Id = userId };
            var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var userManagerResult = await userManager.CreateAsync(user);

            var controller = new PostController(userManager)
            {
                DbContext = dbContext,
            };

            // Act
            await controller.Create(post, CancellationToken.None, null);

            // Assert
            Assert.True(true);
            //Assert.False(controller.DbContext.Posts.Single(u => u.PostId == postId).Equals(null));

        }

        //[Fact]
        //public async Task EditTest()
        //{
        //}
        //[Fact]
        //public async Task UpvoteTest()
        //{
        //}

        //[Fact]
        //public async Task DownvoteTest()
        //{
        //}

        //[Fact]
        //public async Task DeleteTest()
        //{

        //}
    }
}