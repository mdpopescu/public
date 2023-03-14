using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace CustomDialogs.Library
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class Native
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public readonly IntPtr hwnd;
            public readonly uint message;
            public readonly UIntPtr wParam;
            public readonly IntPtr lParam;
            public readonly int time;
            public readonly POINT pt;
            private readonly int lPrivate;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [Flags]
        public enum CLSCTX : uint
        {
            CLSCTX_INPROC_SERVER = 0x1,
            CLSCTX_INPROC_HANDLER = 0x2,
            CLSCTX_LOCAL_SERVER = 0x4,
            CLSCTX_INPROC_SERVER16 = 0x8,
            CLSCTX_REMOTE_SERVER = 0x10,
            CLSCTX_INPROC_HANDLER16 = 0x20,
            CLSCTX_RESERVED1 = 0x40,
            CLSCTX_RESERVED2 = 0x80,
            CLSCTX_RESERVED3 = 0x100,
            CLSCTX_RESERVED4 = 0x200,
            CLSCTX_NO_CODE_DOWNLOAD = 0x400,
            CLSCTX_RESERVED5 = 0x800,
            CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
            CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
            CLSCTX_NO_FAILURE_LOG = 0x4000,
            CLSCTX_DISABLE_AAA = 0x8000,
            CLSCTX_ENABLE_AAA = 0x10000,
            CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
            CLSCTX_ACTIVATE_32_BIT_SERVER = 0x40000,
            CLSCTX_ACTIVATE_64_BIT_SERVER = 0x80000,
            CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
            CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
            CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MULTI_QI
        {
            //[MarshalAs(UnmanagedType.LPStruct)] public Guid pIID;
            public IntPtr pIID;
            [MarshalAs(UnmanagedType.Interface)] public object pItf;
            public int hr;
        }

        // https://msdn.microsoft.com/en-us/library/windows/desktop/aa364962(v=vs.85).aspx
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint GetFinalPathNameByHandle(IntPtr hFile, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszFilePath, uint cchFilePath, uint dwFlags);

        // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363858(v=vs.85).aspx
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateFileW(
            string filename,
            uint desiredAccess,
            uint shareMode,
            IntPtr securityAttributes,
            uint creationDisposition,
            uint flagsAndAttributes,
            IntPtr templateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(
            IntPtr hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteFile(
            IntPtr hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMessage(
            out MSG lpMsg,
            IntPtr hWnd,
            uint wMsgFilterMin,
            uint wMsgFilterMax);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateWindow(
            string lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
            uint dwExStyle,
            string lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        //HRESULT CoCreateInstance(
        //    [in] REFCLSID rclsid,
        //[in] LPUNKNOWN pUnkOuter,
        //[in] DWORD dwClsContext,
        //[in] REFIID riid,
        //[out] LPVOID* ppv
        //);

        //[DllImport("ole32.dll")]
        //public static extern uint CoCreateInstance(
        //    ref Guid clsid,
        //    [MarshalAs(UnmanagedType.IUnknown)] object inner,
        //    uint context,
        //    ref Guid uuid,
        //    [MarshalAs(UnmanagedType.IUnknown)] out object rReturnedComObject);

        [DllImport("ole32.dll", PreserveSig = true)]
        internal static extern int CoCreateInstance(
            ref Guid clsid,
            IntPtr ignore1,
            uint ignore2,
            ref Guid iid,
            [MarshalAs(UnmanagedType.IUnknown), Out] out object pUnkOuter);

        /*
         HRESULT CoCreateInstanceEx(
            [in]      REFCLSID     Clsid,
            [in]      IUnknown     *punkOuter,
            [in]      DWORD        dwClsCtx,
            [in]      COSERVERINFO *pServerInfo,
            [in]      DWORD        dwCount,
            [in, out] MULTI_QI     *pResults
         );
        */

        [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        public static extern int CoCreateInstanceEx(
            [In, MarshalAs(UnmanagedType.LPStruct)]
            Guid rclsid,
            [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
            CLSCTX dwClsCtx,
            IntPtr pServerInfo,
            uint cmq,
            [In, Out] MULTI_QI[] pResults);

        //[DllImport("ole32.dll", PreserveSig = true)]
        //internal static extern int CoCreateInstanceEx(
        //    ref Guid clsid,
        //    IntPtr ignore1,
        //    CLSCTX dwClsCtx,
        //    IntPtr ignore2,
        //    uint ignore3,
        //    [In, Out] MULTI_QI[] pResults);
    }
}