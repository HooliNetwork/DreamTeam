using Hooli.Models;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.Test
{
    public class FeedComponentTest
    {
        private readonly IServiceProvider _serviceProvider;

        public FeedComponentTest()
        {
            var services = new ServiceCollection();

            services.AddEntityFramework()
                      .AddInMemoryStore()
                      .AddDbContext<HooliContext>();

            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
