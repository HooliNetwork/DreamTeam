﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Http.Core;
using Microsoft.AspNet.Http.Core.Authentication;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;
using Hooli.Models;
using Xunit;

namespace Hooli.Controllers
{
    public class ManageControllerTest
    {
        private readonly IServiceProvider _serviceProvider;

        public ManageControllerTest()
        {
            var services = new ServiceCollection();
            services.AddEntityFramework()
                    .AddInMemoryStore()
                    .AddDbContext<HooliContext>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<HooliContext>();

            // IHttpContextAccessor is required for SignInManager, and UserManager
            services.AddInstance<IHttpContextAccessor>(
                new HttpContextAccessor()
                    {
                        HttpContext = new TestHttpContext(),
                    });

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task Index_ReturnsViewBagMessagesExpected()
        {
            // Arrange
            var userId = "TestUserA";
            var phone = "abcdefg";
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };

            var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var userManagerResult = await userManager.CreateAsync(
                new ApplicationUser { Id = userId, UserName = "Test", TwoFactorEnabled = true, PhoneNumber = phone },
                "Pass@word1");
            Assert.True(userManagerResult.Succeeded);

            var signInManager = _serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();

            var httpContext = _serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var controller = new ManageController()
            {
                UserManager = userManager,
                SignInManager = signInManager,
            };
            controller.ActionContext.HttpContext = httpContext;

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);

            Assert.Empty(controller.ViewBag.StatusMessage);

            Assert.NotNull(viewResult.ViewData);
            var model = Assert.IsType<IndexViewModel>(viewResult.ViewData.Model);
            Assert.True(model.TwoFactor);
            Assert.Equal(phone, model.PhoneNumber);
            Assert.True(model.HasPassword);
        }

        private class TestHttpContext : DefaultHttpContext
        {
            public override Task<AuthenticationResult>
                AuthenticateAsync(string authenticationScheme)
            {
                return
                    Task.FromResult(new AuthenticateContext(authenticationScheme).Result);
            }
        }
    }
}