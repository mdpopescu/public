using System.Configuration;
using System.Web.Mvc;
using Munq.MVC3;
using Renfield.Inventory.App_Start;
using Renfield.Inventory.Data;
using WebActivator;

[assembly: PreApplicationStartMethod(typeof (MunqMvc3Startup), "PreStart")]

namespace Renfield.Inventory.App_Start
{
  public static class MunqMvc3Startup
  {
    public static void PreStart()
    {
      DependencyResolver.SetResolver(new MunqDependencyResolver());

      var ioc = MunqDependencyResolver.Container;

      // database stuff
      ioc.Register("InventoryDB", c => ConfigurationManager.ConnectionStrings["InventoryDB"].ConnectionString);
      ioc.Register<Repository>(c => new InventoryDB(c.Resolve<string>("InventoryDB")));
    }
  }
}