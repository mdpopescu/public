using System.Windows.Forms;

namespace Renfield.VideoSpinner
{
  public class WaitGuard : Guard
  {
    public WaitGuard(Control control)
      : base(() => control.UseWaitCursor = true, () => control.UseWaitCursor = false)
    {
    }
  }
}