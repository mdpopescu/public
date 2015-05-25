using System;
using System.Windows.Forms;

namespace TaskSpikes
{
  public static class ControlExtensions
  {
    /// <summary>
    ///   Use to safely perform an action on a control from a background thread.
    /// </summary>
    /// <param name="control"> Control to check. </param>
    /// <param name="action"> Action that should involve that control (and ONLY that one). </param>
    public static void UIChange(this Control control, Action action)
    {
      if (control.InvokeRequired)
      {
        control.Invoke(action);
      }
      else
      {
        action();
      }
    }
  }
}