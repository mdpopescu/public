using System.Web.Mvc;
using Munq.MVC3;
using Budget.Services;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Budget.App_Start.MunqMvc3Startup), "PreStart")]

namespace Budget.App_Start
{
  public static class MunqMvc3Startup
  {
    public static void PreStart()
    {
      DependencyResolver.SetResolver(new MunqDependencyResolver());

      var ioc = MunqDependencyResolver.Container;
      ioc.Register<Logic>(c => new BusinessLogic());
    }
  }
}
