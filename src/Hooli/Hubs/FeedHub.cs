using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Hooli.Models;
using System.Collections.Concurrent;

namespace Hooli.Hubs
{
    [HubName("Feed")]
    public class FeedHub : Hub
    {
    }
}