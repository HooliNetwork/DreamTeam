using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.SqlServer;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;

namespace Hooli.Models
{
    public static class SampleData
    {
        const string imgUrl = "~/Images/placeholder.png";
        const string defaultAdminUserName = "Tester";
        const string defaultAdminPassword = "BlaBla1234!!";

        public static async Task InitializeHooliDatabaseAsync(IServiceProvider serviceProvider, bool createUsers = true)
        {
            using (var db = serviceProvider.GetService<HooliContext>())
            {
                var sqlServerDatabase = db.Database as SqlServerDatabase;
                if (sqlServerDatabase != null)
                {    
                    if (createUsers)
                    {
                        await CreateUser(serviceProvider);
                    }
                    await InsertTestData(serviceProvider);
                }
            }
        }

        private static async Task InsertTestData(IServiceProvider serviceProvider)
        {
            var posts = GetPosts();
            await AddOrUpdateAsync(serviceProvider, a => a.PostId, posts);
        }

        // TODO [EF] This may be replaced by a first class mechanism in EF
        private static async Task AddOrUpdateAsync<TEntity>(
            IServiceProvider serviceProvider,
            Func<TEntity, object> propertyToMatch, IEnumerable<TEntity> entities)
            where TEntity : class
        {
            // Query in a separate context so that we can attach existing entities as modified
            List<TEntity> existingData;
            using (var db = serviceProvider.GetService<HooliContext>())
            {
                existingData = db.Set<TEntity>().ToList();
            }

            using (var db = serviceProvider.GetService<HooliContext>())
            {
                foreach (var item in entities)
                {
                    db.Entry(item).State = existingData.Any(g => propertyToMatch(g).Equals(propertyToMatch(item)))
                        ? EntityState.Modified
                        : EntityState.Added;
                }

                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Creates a store manager user who can manage the inventory.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private static async Task CreateUser(IServiceProvider serviceProvider)
        {
            var configuration = new Configuration()
                        .AddJsonFile("config.json")
                        .AddEnvironmentVariables();

            //const string adminRole = "Administrator";

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            // TODO: Identity SQL does not support roles yet
            //var roleManager = serviceProvider.GetService<ApplicationRoleManager>();
            //if (!await roleManager.RoleExistsAsync(adminRole))
            //{
            //    await roleManager.CreateAsync(new IdentityRole(adminRole));
            //}

            var user = await userManager.FindByNameAsync(configuration.Get<string>(defaultAdminUserName));
            if (user == null)
            {
                user = new ApplicationUser { UserName = configuration.Get<string>(defaultAdminUserName) };
                await userManager.CreateAsync(user, configuration.Get<string>(defaultAdminPassword));
                await userManager.AddClaimAsync(user, new Claim("ManageGroup", "Allowed"));
            }
        }

        private static Post[] GetPosts()
        {
              var posts = new Post[]
              {



              };

              return posts;
        }
    }
}