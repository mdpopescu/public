using System.Web.Mvc;
using Munq.MVC3;
using Renfield.SafeRedir.App_Start;
using Renfield.SafeRedir.Services;
using WebActivator;

[assembly: PreApplicationStartMethod(typeof (MunqMvc3Startup), "PreStart")]

namespace Renfield.SafeRedir.App_Start
{
  public static class MunqMvc3Startup
  {
    public static void PreStart()
    {
      DependencyResolver.SetResolver(new MunqDependencyResolver());

      var ioc = MunqDependencyResolver.Container;
      ioc.Register<ShorteningService>(c => new DbShorteningService());
    }
  }
}