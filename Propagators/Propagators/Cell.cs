using System;
using System.Threading.Tasks;

namespace Propagators
{
  public class Cell
  {
    public event EventHandler ValueChanged;

    public object Value { get; private set; }

    public async Task SetValueAsync(object value)
    {
      Value = value;

      await Task.Yield();

      ValueChanged?.Invoke(this, null);
    }

    //
  }
}