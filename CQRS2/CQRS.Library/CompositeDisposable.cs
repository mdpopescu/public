using System;
using System.Collections.Generic;

namespace CQRS.Library
{
  public class CompositeDisposable : IDisposable
  {
    public CompositeDisposable(IEnumerable<IDisposable> list)
    {
      this.list = list;
    }

    public void Dispose()
    {
      foreach (var disposable in list)
        disposable.Dispose();
    }

    //

    private readonly IEnumerable<IDisposable> list;
  }
}