using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Renfield.Inventory
{
  public class LiveUpdateHub : Hub
  {
    public static readonly Lazy<IHubConnectionContext<dynamic>> INSTANCE =
      new Lazy<IHubConnectionContext<dynamic>>(() => GlobalHost.ConnectionManager.GetHubContext<LiveUpdateHub>().Clients);
  }
}