using System.Collections;
using System.Collections.Generic;

namespace BigDataProcessing.Library.Services
{
  public class ConcurrentEnumerator<T> : IEnumerator<T>
  {
    public ConcurrentEnumerator(IEnumerator<T> enumerator)
    {
      this.enumerator = enumerator;
    }

    public void Dispose()
    {
      enumerator.Dispose();
    }

    public bool MoveNext()
    {
      lock (gate)
        return enumerator.MoveNext();
    }

    public void Reset()
    {
      lock (gate)
        enumerator.Reset();
    }

    public T Current
    {
      get
      {
        lock (gate)
          return enumerator.Current;
      }
    }

    object IEnumerator.Current
    {
      get { return Current; }
    }

    //

    private readonly object gate = new object();

    private readonly IEnumerator<T> enumerator;
  }
}