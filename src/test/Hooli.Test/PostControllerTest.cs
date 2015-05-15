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

            var controller = new PostController()
            {
                DbContext = dbContext,
            };

            // Act
            await controller.Create(post, CancellationToken.None, null);

            // Assert
            Assert.False(controller.DbContext.Posts.Single(u => u.PostId == postId).Equals(null));

        }

        [Fact]
        public async Task EditTest()
        {
            // Arrange
            var postId = 1;
            var post = new Post() { PostId = postId, Points = 0, Title = "Text1" };

            var changedPost = new Post() { PostId = postId, Points = 0, Title = "Text2" };

            var dbContext = _serviceProvider.GetRequiredService<HooliContext>();

            var userId = "1";
            var user = new ApplicationUser() { UserName = "Test", Id = userId };
            var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var userManagerResult = await userManager.CreateAsync(user);

            var controller = new PostController()
            {
                DbContext = dbContext,
            };

            // Act 
            await controller.Edit(changedPost);

            // Assert 
            Assert.True(post.Title == changedPost.Title);
            Assert.True(post.Text == changedPost.Text);
            Assert.True(post.Points == changedPost.Points);
            Assert.True(post.Image == changedPost.Image);
            Assert.True(post.Link == changedPost.Link);
        }

        [Fact]
        public async Task PointsTest()
        {
            // Arrange
            var postId = 1;
            var points = 0;
            var post = new Post() { PostId = postId, Points = points };
            var dbContext = _serviceProvider.GetRequiredService<HooliContext>();

            var userId = "1";
            var user = new ApplicationUser() { UserName = "Test", Id = userId };
            var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var userManagerResult = await userManager.CreateAsync(user);

            var controller = new PostController()
            {
                DbContext = dbContext,
            };

            // Act Upvote
            await controller.Upvote(post, CancellationToken.None);

            // Assert Upvote
            Assert.True(post.Points == 1);

            // Act Downvote
            await controller.Downvote(post, CancellationToken.None);

            // Assert Downvote
            Assert.True(post.Points == 0);
        }



        [Fact]
        public async Task DeleteTest()
        {
            // Arrange
            var postId = 1;
            var post = new Post() { PostId = postId, Points = 0 };
            var dbContext = _serviceProvider.GetRequiredService<HooliContext>();

            var userId = "1";
            var user = new ApplicationUser() { UserName = "Test", Id = userId };
            var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var userManagerResult = await userManager.CreateAsync(user);

            dbContext.Add(post);
            dbContext.SaveChanges();

            var controller = new PostController()
            {
                DbContext = dbContext,
            };

            // Act 
            await controller.Delete(post, CancellationToken.None);

            // Assert 
            Assert.True(controller.DbContext.Posts.Single(u => u.PostId == postId).Equals(null));
        }
    }
}