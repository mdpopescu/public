using System.Windows.Forms;

namespace Renfield.VideoSpinner
{
    public class WaitGuard : Guard
    {
        public WaitGuard()
            : base(() => Cursor.Current = Cursors.WaitCursor, () => Cursor.Current = Cursors.Default)
        {
        }
    }
}