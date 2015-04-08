using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using Renfield.Inventory.App_Start;
using Renfield.Inventory.Data;
using Renfield.Inventory.Services;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using WebActivator;

[assembly: PostApplicationStartMethod(typeof (SimpleInjectorInitializer), "Initialize")]

namespace Renfield.Inventory.App_Start
{
  public static class SimpleInjectorInitializer
  {
    /// <summary>Initialize the container and register it as MVC Dependency Resolver.</summary>
    public static void Initialize()
    {
      // Did you know the container can diagnose your configuration? Go to: https://bit.ly/YE8OJj.
      var container = new Container();

      InitializeContainer(container);

      container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

      container.Verify();

      DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
    }

    private static void InitializeContainer(Container container)
    {
      // database stuff
      container.Register<Repository>(() => new InventoryDB(ConfigurationManager.ConnectionStrings["InventoryDB"].ConnectionString));
      container.Register<Logic>(() => new BusinessLogic(container.GetInstance<Repository>));
    }
  }
}