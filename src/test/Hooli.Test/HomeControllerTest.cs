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

        //[Fact]
        //public void Error_ReturnsErrorView()
        //{
        //    // Arrange
        //    var controller = new HomeController();
        //    var errorView = "~/Views/Shared/Error.cshtml";

        //    // Act
        //    var result = controller.Error();

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);

        //    Assert.Equal(errorView, viewResult.ViewName);
        //}

        

        //[Fact]
        //public void StatusCodePage_ReturnsStatusCodePage()
        //{
        //    // Arrange
        //    var controller = new HomeController();
        //    var statusCodeView = "~/Views/Shared/StatusCodePage.cshtml";

        //    // Action
        //    var result = controller.StatusCodePage();

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);

        //    Assert.Equal(statusCodeView, viewResult.ViewName);
        //}

        // Finish when Search has been completed
        //[Fact]
        //public async Task SearchTest()
        //{
        //    // Arrange
        //    var user1 = new ApplicationUser() { Id = "1", UserName = "name1", FirstName = "Benedikt", LastName = "Sigurleifsson" };
        //    var user2 = new ApplicationUser() { Id = "2", UserName = "name2", FirstName = "Magnus", LastName = "Magnusson" };
        //    var user3 = new ApplicationUser() { Id = "3", UserName = "name3", FirstName = "Sara", LastName = "Arnadottir" };

        //    var group1 = new Group() { GroupId = "1", GroupName = "Bikes" };
        //    var group2 = new Group() { GroupId = "2", GroupName = "Cars" };
        //    var group3 = new Group() { GroupId = "3", GroupName = "Skates" };


        //    var dbContext = _serviceProvider.GetRequiredService<HooliContext>();
        //    dbContext.Add(user1);
        //    dbContext.Add(user2);
        //    dbContext.Add(user3);
        //    dbContext.Add(group1);
        //    dbContext.Add(group2);
        //    dbContext.Add(group3);
        //    dbContext.SaveChanges();

        //    var controller = new HomeController()
        //    {
        //        DbContext = dbContext,
        //    };

        //    // Act
        //    await controller.Search();

        //    // Assert
        //    Assert.False(controller.DbContext.Groups.Single(u => u.GroupId == groupId).Equals(null));
        //}


    }
}