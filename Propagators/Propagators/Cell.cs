using System;

namespace Propagators
{
  public class Cell
  {
    public event EventHandler ValueChanged;

    public object Value { get; private set; }

    public void SetValue(object value)
    {
      if (Value == value)
        return;

      Value = value;
      ValueChanged?.Invoke(this, null);
    }

    //
  }
}