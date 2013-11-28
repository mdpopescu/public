using Budget.Controllers;
using Budget.Services;
using Microsoft.Owin;
using Munq;
using Owin;
using System.Web.Mvc;

[assembly: OwinStartupAttribute(typeof(Budget.Startup))]
namespace Budget
{
  public partial class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      ConfigureAuth(app);
    }
  }
}
