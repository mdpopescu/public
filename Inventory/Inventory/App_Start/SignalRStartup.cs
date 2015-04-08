using Microsoft.Owin;
using Owin;
using Renfield.Inventory.App_Start;

[assembly: OwinStartup(typeof (SignalRStartup))]

namespace Renfield.Inventory.App_Start
{
  public class SignalRStartup
  {
    public void Configuration(IAppBuilder app)
    {
      // Any connection or hub wire up and configuration should go here
      app.MapSignalR();
    }
  }
}