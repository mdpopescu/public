using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Renfield.Inventory
{
  public class LiveUpdateHub : Hub
  {
    public static readonly Lazy<IHubConnectionContext> Instance =
      new Lazy<IHubConnectionContext>(() => GlobalHost.ConnectionManager.GetHubContext<LiveUpdateHub>().Clients);
  }
}