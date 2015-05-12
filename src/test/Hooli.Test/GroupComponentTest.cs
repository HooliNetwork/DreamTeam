using Hooli.Models;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.Test
{
    public class GroupComponentTest
    {
        private readonly IServiceProvider _serviceProvider;

        public GroupComponentTest()
        {
            var services = new ServiceCollection();

            services.AddEntityFramework()
                      .AddInMemoryStore()
                      .AddDbContext<HooliContext>();

            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
