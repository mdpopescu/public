using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskSpikes
{
  public class TaskRunner<T>
  {
    public TaskRunner(Func<CancellationToken, T> func)
    {
      this.func = func;

      cts = new CancellationTokenSource();
    }

    // ReSharper disable once UnusedMethodReturnValue.Global
    public Task<T> Start()
    {
      Cancel();

      return Task.Run(() =>
      {
        try
        {
          return func(cts.Token);
        }
        catch (OperationCanceledException)
        {
          return default(T);
        }
      });
    }

    public void Cancel()
    {
      cts.Cancel();
      cts = new CancellationTokenSource();
    }

    //

    private readonly Func<CancellationToken, T> func;

    private CancellationTokenSource cts;
  }
}