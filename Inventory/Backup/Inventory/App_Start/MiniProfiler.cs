using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Renfield.Inventory.App_Start;
using StackExchange.Profiling;
using StackExchange.Profiling.MVCHelpers;
using StackExchange.Profiling.SqlFormatters;
using WebActivator;

//using StackExchange.Profiling.Data.EntityFramework;
//using StackExchange.Profiling.Data.Linq2Sql;

[assembly: WebActivator.PreApplicationStartMethod(typeof (MiniProfilerPackage), "PreStart")]
[assembly: PostApplicationStartMethod(typeof (MiniProfilerPackage), "PostStart")]

namespace Renfield.Inventory.App_Start
{
  public static class MiniProfilerPackage
  {
    public static void PreStart()
    {
      // Be sure to restart you ASP.NET Developement server, this code will not run until you do that. 

      MiniProfiler.Settings.SqlFormatter = new SqlServerFormatter();

      //Make sure the MiniProfiler handles BeginRequest and EndRequest
      DynamicModuleUtility.RegisterModule(typeof (MiniProfilerStartupModule));

      //Setup profiler for Controllers via a Global ActionFilter
      GlobalFilters.Filters.Add(new ProfilingActionFilter());

      // You can use this to check if a request is allowed to view results
      //MiniProfiler.Settings.Results_Authorize = (request) =>
      //{
      // you should implement this if you need to restrict visibility of profiling on a per request basis 
      //    return !DisableProfilingResults; 
      //};

      // the list of all sessions in the store is restricted by default, you must return true to alllow it
      MiniProfiler.Settings.Results_List_Authorize = (request) =>
      {
        //you may implement this if you need to restrict visibility of profiling lists on a per request basis 
        return true; // all requests are kosher
      };
    }

    public static void PostStart()
    {
      // Intercept ViewEngines to profile all partial views and regular views.
      // If you prefer to insert your profiling blocks manually you can comment this out
      var copy = ViewEngines.Engines.ToList();
      ViewEngines.Engines.Clear();
      foreach (var item in copy)
      {
        ViewEngines.Engines.Add(new ProfilingViewEngine(item));
      }
    }
  }

  public class MiniProfilerStartupModule : IHttpModule
  {
    public void Init(HttpApplication context)
    {
      context.BeginRequest += (sender, e) =>
      {
        var request = ((HttpApplication) sender).Request;
        //TODO: By default only local requests are profiled, optionally you can set it up
        //  so authenticated users are always profiled
        //if (request.IsLocal)
        {
          MiniProfiler.Start();
        }
      };


      // TODO: You can control who sees the profiling information
      /*
            context.AuthenticateRequest += (sender, e) =>
            {
                if (!CurrentUserIsAllowedToSeeProfiler())
                {
                    StackExchange.Profiling.MiniProfiler.Stop(discardResults: true);
                }
            };
            */

      context.EndRequest += (sender, e) => { MiniProfiler.Stop(); };
    }

    public void Dispose()
    {
    }
  }
}