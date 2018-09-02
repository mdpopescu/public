using System;
using System.Windows.Forms;

namespace ExtractLinks.Services
{
    public class BusyGuard : IDisposable
    {
        public BusyGuard()
        {
            Cursor.Current = Cursors.WaitCursor;
        }

        public void Dispose()
        {
            Cursor.Current = Cursors.Default;
        }
    }
}