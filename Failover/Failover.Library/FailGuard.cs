using System;

namespace Renfield.Failover.Library
{
  public class FailGuard
  {
    public FailGuard(Logger logger)
    {
      this.logger = logger;
    }

    public void Attempt(params Action[] actions)
    {
      foreach (var action in actions)
      {
        try
        {
          action.Invoke();
          logger.Debug("Success.");
          break;
        }
        catch (Exception ex)
        {
          logger.Error(ex);
        }
      }
    }

    //

    private readonly Logger logger;
  }
}