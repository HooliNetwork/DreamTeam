using Hooli.Models;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.Test
{
    public class EventComponentTest
    {
        private readonly IServiceProvider _serviceProvider;

        public EventComponentTest()
        {
            var services = new ServiceCollection();

            services.AddEntityFramework()
                      .AddInMemoryStore()
                      .AddDbContext<HooliContext>();

            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
