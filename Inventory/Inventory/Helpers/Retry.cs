using System;
using System.Threading;

namespace Renfield.Inventory.Helpers
{
  public class Retry
  {
    /// <summary>
    ///   Try an action up to <paramref name="count">count</paramref> times
    /// </summary>
    /// <param name="count"> Maximum number of tries </param>
    /// <param name="delay"> Delay to insert between retries </param>
    /// <param name="action"> Action to invoke that might throw </param>
    public static void Times(int count, TimeSpan delay, Action action)
    {
      if (count < 1)
        count = 1;

      var i = 0;
      do
      {
        try
        {
          action.Invoke();
          return;
        }
        catch (Exception)
        {
          i++;
          if (i == count)
            throw;

          Thread.Sleep(delay);
        }
      } while (true);
    }

    /// <summary>
    ///   Try an action up to <paramref name="count">count</paramref> times
    /// </summary>
    /// <param name="count"> Maximum number of tries </param>
    /// <param name="action"> Action to invoke that might throw </param>
    public static void Times(int count, Action action)
    {
      if (count < 1)
        count = 1;

      var i = 0;
      do
      {
        try
        {
          action.Invoke();
          return;
        }
        catch (Exception)
        {
          i++;
          if (i == count)
            throw;
        }
      } while (true);
    }
  }
}