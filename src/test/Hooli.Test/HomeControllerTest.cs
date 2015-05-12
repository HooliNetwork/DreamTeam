using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Framework.Caching.Memory;
using Microsoft.Framework.DependencyInjection;
using Hooli.Models;
using Xunit;

namespace Hooli.Controllers
{
    public class HomeControllerTest
    {
        private readonly IServiceProvider _serviceProvider;

        public HomeControllerTest()
        {
            var services = new ServiceCollection();

            services.AddEntityFramework()
                      .AddInMemoryStore()
                      .AddDbContext<HooliContext>();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void Error_ReturnsErrorView()
        {
            // Arrange
            var controller = new HomeController();
            var errorView = "~/Views/Shared/Error.cshtml";

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal(errorView, viewResult.ViewName);
        }

        

        [Fact]
        public void StatusCodePage_ReturnsStatusCodePage()
        {
            // Arrange
            var controller = new HomeController();
            var statusCodeView = "~/Views/Shared/StatusCodePage.cshtml";

            // Action
            var result = controller.StatusCodePage();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal(statusCodeView, viewResult.ViewName);
        }

        //[Fact]
        //public async Task GetTopPostsTest()
        //{
        //    // Arrange
        //    var post1 = new Post() { Title = "title1", Text = "text1", Points = 150 };
        //    var post2 = new Post() { Title = "title2", Text = "text2", Points = 300 };
        //    var post3 = new Post() { Title = "title3", Text = "text3", Points = 50 };

        //    var dbContext = _serviceProvider.GetRequiredService<HooliContext>();
        //    dbContext.Add(post1);
        //    dbContext.Add(post2);
        //    dbContext.Add(post3);
        //    dbContext.SaveChanges();

        //    var controller = new HomeController()
        //    {
        //        DbContext = dbContext,
        //    };

        //    // Act
        //    await controller.

        //    // Assert
        //    Assert.False(controller.DbContext.Groups.Single(u => u.GroupId == groupId).Equals(null));
        //}


    }
}