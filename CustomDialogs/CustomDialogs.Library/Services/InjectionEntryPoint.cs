using EasyHook;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace CustomDialogs.Library.Services
{
    /// <summary>
    ///     EasyHook will look for a class implementing <see cref="EasyHook.IEntryPoint" /> during injection. This
    ///     becomes the entry point within the target process after injection is complete.
    /// </summary>
    public class InjectionEntryPoint : IEntryPoint
    {
        /// <summary>
        ///     Reference to the server interface within the main program
        /// </summary>
        private readonly ServerInterface server;

        /// <summary>
        ///     Message queue of all files accessed
        /// </summary>
        private readonly Queue<string> messageQueue = new Queue<string>();

        /// <summary>
        ///     EasyHook requires a constructor that matches <paramref name="context" /> and any additional parameters as provided
        ///     in the original call to
        ///     <see cref="EasyHook.RemoteHooking.Inject(int, EasyHook.InjectionOptions, string, string, object[])" />.
        ///     Multiple constructors can exist on the same <see cref="EasyHook.IEntryPoint" />, providing that each one has a
        ///     corresponding Run method (e.g. <see cref="Run(EasyHook.RemoteHooking.IContext, string)" />).
        /// </summary>
        /// <param name="context">The RemoteHooking context</param>
        /// <param name="channelName">The name of the IPC channel</param>
        public InjectionEntryPoint(RemoteHooking.IContext context, string channelName)
        {
            // Connect to server object using provided channel name
            server = RemoteHooking.IpcConnectClient<ServerInterface>(channelName);

            // If Ping fails then the Run method will be not be called
            server.Ping();
        }

        /// <summary>
        ///     The main entry point for our logic once injected within the target process.
        ///     This is where the hooks will be created, and a loop will be entered until host process exits.
        ///     EasyHook requires a matching Run method for the constructor
        /// </summary>
        /// <param name="context">The RemoteHooking context</param>
        /// <param name="channelName">The name of the IPC channel</param>
        public void Run(RemoteHooking.IContext context, string channelName)
        {
            // Injection is now complete and the server interface is connected
            server.IsInstalled(RemoteHooking.GetCurrentProcessId());

            // Install hooks

            //var getMessageHook = LocalHook.Create(
            //    LocalHook.GetProcAddress("user32.dll", "GetMessageW"),
            //    new GetMessageDelegate(GetMessageHook),
            //    this
            //);

            //var createWindowHook = LocalHook.Create(
            //    LocalHook.GetProcAddress("user32.dll", "CreateWindowW"),
            //    new CreateWindowDelegate(CreateWindowHook),
            //    this
            //);

            //var createWindowExHook = LocalHook.Create(
            //    LocalHook.GetProcAddress("user32.dll", "CreateWindowEx"),
            //    new CreateWindowExDelegate(CreateWindowExHook),
            //    this
            //);

            var coCreateInstanceExHook = LocalHook.Create(
                LocalHook.GetProcAddress("ole32.dll", "CoCreateInstanceEx"),
                new CoCreateInstanceExDelegate(CoCreateInstanceExHook),
                this
            );

            // Activate hooks on all threads except the current thread
            //getMessageHook.ThreadACL.SetExclusiveACL(new[] { 0 });
            //createWindowHook.ThreadACL.SetExclusiveACL(new[] { 0 });
            //createWindowExHook.ThreadACL.SetExclusiveACL(new[] { 0 });
            coCreateInstanceExHook.ThreadACL.SetExclusiveACL(new[] { 0 });

            server.ReportMessage("Hooks installed");

            // Wake up the process (required if using RemoteHooking.CreateAndInject)
            RemoteHooking.WakeUpProcess();

            try
            {
                // Loop until the main program closes (i.e. IPC fails)
                while (true)
                {
                    Thread.Sleep(500);

                    string[] queued;

                    lock (messageQueue)
                    {
                        queued = messageQueue.ToArray();
                        messageQueue.Clear();
                    }

                    // Send newly monitored file accesses to the main program
                    if (queued.Length > 0)
                        server.ReportMessages(queued);
                    else
                        server.Ping();
                }
            }
            catch
            {
                // Ping() or ReportMessages() will raise an exception if host is unreachable
            }

            // Remove hooks
            //getMessageHook.Dispose();
            //createWindowHook.Dispose();
            //createWindowExHook.Dispose();
            coCreateInstanceExHook.Dispose();

            // Finalise cleanup of hooks
            LocalHook.Release();
        }

        #region GetMessage Hook

        /// <summary>
        ///     The GetMessage delegate, this is needed to create a delegate of our hook function
        ///     <see cref="InjectionEntryPoint.GetMessageHook" />.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private delegate bool GetMessageDelegate(
            out Native.MSG lpMsg,
            IntPtr hWnd,
            uint wMsgFilterMin,
            uint wMsgFilterMax);

        /// <summary>
        ///     The GetMessage hook function. This will be called instead of the original GetMessage once hooked.
        /// </summary>
        private bool GetMessageHook(
            out Native.MSG lpMsg,
            IntPtr hWnd,
            uint wMsgFilterMin,
            uint wMsgFilterMax)
        {
            // Call original first so we have a value for lpMsg
            var result = Native.GetMessage(out lpMsg, hWnd, wMsgFilterMin, wMsgFilterMax);

            Log($"GETMESSAGE message={lpMsg.message} wParam={lpMsg.wParam} lParam={lpMsg.lParam}");

            return result;
        }

        #endregion

        #region CreateWindow Hook

        /// <summary>
        ///     The CreateWindow delegate, this is needed to create a delegate of our hook function
        ///     <see cref="InjectionEntryPoint.CreateWindowHook" />.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate IntPtr CreateWindowDelegate(
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

        /// <summary>
        ///     The CreateWindow hook function. This will be called instead of the original CreateWindow once hooked.
        /// </summary>
        private IntPtr CreateWindowHook(
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
            IntPtr lpParam)
        {
            Log($"CreateWindow class name={lpClassName} window name={lpWindowName}");

            // now call the original
            return Native.CreateWindow(lpClassName, lpWindowName, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);
        }

        #endregion

        #region CreateWindowEx Hook

        /// <summary>
        ///     The CreateWindowEx delegate, this is needed to create a delegate of our hook function
        ///     <see cref="InjectionEntryPoint.CreateWindowExHook" />.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate IntPtr CreateWindowExDelegate(
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

        /// <summary>
        ///     The CreateWindowEx hook function. This will be called instead of the original CreateWindowEx once hooked.
        /// </summary>
        private IntPtr CreateWindowExHook(
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
            IntPtr lpParam)
        {
            Log($"CreateWindowEx style={dwExStyle} class name={lpClassName} window name={lpWindowName}");

            // now call the original
            return Native.CreateWindowEx(dwExStyle, lpClassName, lpWindowName, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);
        }

        #endregion

        #region CoCreateInstanceEx Hook

        /// <summary>
        ///     The GetMessage delegate, this is needed to create a delegate of our hook function
        ///     <see cref="InjectionEntryPoint.CoCreateInstanceExHook" />.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CoCreateInstanceExDelegate(
            [In, MarshalAs(UnmanagedType.LPStruct)]
            Guid rclsid,
            [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
            Native.CLSCTX dwClsCtx,
            IntPtr pServerInfo,
            uint cmq,
            [In, Out] Native.MULTI_QI[] pResults);

        /// <summary>
        ///     The CoCreateInstanceEx hook function. This will be called instead of the original CoCreateInstanceEx once hooked.
        /// </summary>
        private int CoCreateInstanceExHook(
            [In, MarshalAs(UnmanagedType.LPStruct)]
            Guid rclsid,
            [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
            Native.CLSCTX dwClsCtx,
            IntPtr pServerInfo,
            uint cmq,
            [In, Out] Native.MULTI_QI[] pResults)
        {
            Log($"CoCreateInstanceEx rclsid={rclsid}");

            // now call the original
            return Native.CoCreateInstanceEx(rclsid, pUnkOuter, dwClsCtx, pServerInfo, cmq, pResults);
        }

        #endregion

        private void Log(string message)
        {
            try
            {
                lock (messageQueue)
                {
                    if (messageQueue.Count < 1000)
                        // Add message to send to the main program
                        messageQueue.Enqueue(
                            $"[{RemoteHooking.GetCurrentProcessId()}:{RemoteHooking.GetCurrentThreadId()}]: {message}");
                }
            }
            catch (Exception ex)
            {
                lock (messageQueue)
                {
                    messageQueue.Enqueue($"*** ERROR *** {ex}");
                }

                // swallow exceptions so that any issues caused by this code do not crash target process
            }
        }
    }
}