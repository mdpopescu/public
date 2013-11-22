using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace TheGame.Hubs
{
  public class GameHub : Hub
  {
    public void Select(object p)
    {
      Clients.All.select(p);
    }
  }
}