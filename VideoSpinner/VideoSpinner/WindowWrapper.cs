using System;
using System.Windows.Forms;

namespace Renfield.VideoSpinner
{
    /// <summary>
    /// Creates IWin32Window around an IntPtr
    /// </summary>
    public class WindowWrapper : IWin32Window
    {
        /// <summary>
        /// Original ptr
        /// </summary>
        public IntPtr Handle
        {
            get { return hwnd; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="handle">Handle to wrap</param>
        public WindowWrapper(IntPtr handle)
        {
            hwnd = handle;
        }

        //

        private readonly IntPtr hwnd;
    }
}