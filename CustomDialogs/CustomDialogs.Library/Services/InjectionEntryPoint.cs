using EasyHook;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
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

            // GetMessage https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getmessagew
            var getMessageHook = LocalHook.Create(
                LocalHook.GetProcAddress("user32.dll", "GetMessageW"),
                new GetMessageDelegate(GetMessageHook),
                this
            );

            // CreateFile https://msdn.microsoft.com/en-us/library/windows/desktop/aa363858(v=vs.85).aspx
            var createFileHook = LocalHook.Create(
                LocalHook.GetProcAddress("kernel32.dll", "CreateFileW"),
                new CreateFileDelegate(CreateFileHook),
                this);

            // ReadFile https://msdn.microsoft.com/en-us/library/windows/desktop/aa365467(v=vs.85).aspx
            var readFileHook = LocalHook.Create(
                LocalHook.GetProcAddress("kernel32.dll", "ReadFile"),
                new ReadFileDelegate(ReadFileHook),
                this);

            // WriteFile https://msdn.microsoft.com/en-us/library/windows/desktop/aa365747(v=vs.85).aspx
            var writeFileHook = LocalHook.Create(
                LocalHook.GetProcAddress("kernel32.dll", "WriteFile"),
                new WriteFileDelegate(WriteFileHook),
                this);

            // Activate hooks on all threads except the current thread
            getMessageHook.ThreadACL.SetExclusiveACL(new[] { 0 });
            createFileHook.ThreadACL.SetExclusiveACL(new[] { 0 });
            readFileHook.ThreadACL.SetExclusiveACL(new[] { 0 });
            writeFileHook.ThreadACL.SetExclusiveACL(new[] { 0 });

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
            getMessageHook.Dispose();
            createFileHook.Dispose();
            readFileHook.Dispose();
            writeFileHook.Dispose();

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
            var result = Native.GetMessageW(out lpMsg, hWnd, wMsgFilterMin, wMsgFilterMax);

            try
            {
                lock (messageQueue)
                {
                    if (messageQueue.Count < 1000)
                        // Add message to send to the main program
                        messageQueue.Enqueue(
                            $"[{RemoteHooking.GetCurrentProcessId()}:{RemoteHooking.GetCurrentThreadId()}]: GETMESSAGE message={lpMsg.message} wParam={lpMsg.wParam} lParam={lpMsg.lParam}");
                }
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            return result;
        }

        #endregion

        #region CreateFileW Hook

        /// <summary>
        ///     The CreateFile delegate, this is needed to create a delegate of our hook function
        ///     <see cref="InjectionEntryPoint.CreateFileHook" />.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr CreateFileDelegate(
            string filename,
            uint desiredAccess,
            uint shareMode,
            IntPtr securityAttributes,
            uint creationDisposition,
            uint flagsAndAttributes,
            IntPtr templateFile);

        /// <summary>
        ///     The CreateFile hook function. This will be called instead of the original CreateFile once hooked.
        /// </summary>
        private IntPtr CreateFileHook(
            string filename,
            uint desiredAccess,
            uint shareMode,
            IntPtr securityAttributes,
            uint creationDisposition,
            uint flagsAndAttributes,
            IntPtr templateFile)
        {
            try
            {
                lock (messageQueue)
                {
                    if (messageQueue.Count < 1000)
                    {
                        var mode = string.Empty;
                        switch (creationDisposition)
                        {
                            case 1:
                                mode = "CREATE_NEW";
                                break;
                            case 2:
                                mode = "CREATE_ALWAYS";
                                break;
                            case 3:
                                mode = "OPEN_ALWAYS";
                                break;
                            case 4:
                                mode = "OPEN_EXISTING";
                                break;
                            case 5:
                                mode = "TRUNCATE_EXISTING";
                                break;
                        }

                        // Add message to send to the main program
                        messageQueue.Enqueue($"[{RemoteHooking.GetCurrentProcessId()}:{RemoteHooking.GetCurrentThreadId()}]: CREATE ({mode}) \"{filename}\"");
                    }
                }
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            // now call the original API...
            return Native.CreateFileW(
                filename,
                desiredAccess,
                shareMode,
                securityAttributes,
                creationDisposition,
                flagsAndAttributes,
                templateFile);
        }

        #endregion

        #region ReadFile Hook

        /// <summary>
        ///     The ReadFile delegate, this is needed to create a delegate of our hook function
        ///     <see cref="InjectionEntryPoint.ReadFileHook" />.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate bool ReadFileDelegate(
            IntPtr hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        /// <summary>
        ///     The ReadFile hook function. This will be called instead of the original ReadFile once hooked.
        /// </summary>
        private bool ReadFileHook(
            IntPtr hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped)
        {
            lpNumberOfBytesRead = 0;

            // Call original first so we have a value for lpNumberOfBytesRead
            var result = Native.ReadFile(hFile, lpBuffer, nNumberOfBytesToRead, out lpNumberOfBytesRead, lpOverlapped);

            try
            {
                lock (messageQueue)
                {
                    if (messageQueue.Count < 1000)
                    {
                        // Retrieve filename from the file handle
                        var filename = new StringBuilder(255);
                        Native.GetFinalPathNameByHandle(hFile, filename, 255, 0);

                        // Add message to send to the main program
                        messageQueue.Enqueue($"[{RemoteHooking.GetCurrentProcessId()}:{RemoteHooking.GetCurrentThreadId()}]: READ ({lpNumberOfBytesRead} bytes) \"{filename}\"");
                    }
                }
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            return result;
        }

        #endregion

        #region WriteFile Hook

        /// <summary>
        ///     The WriteFile delegate, this is needed to create a delegate of our hook function
        ///     <see cref="InjectionEntryPoint.WriteFileHook" />.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private delegate bool WriteFileDelegate(
            IntPtr hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        /// <summary>
        ///     The WriteFile hook function. This will be called instead of the original WriteFile once hooked.
        /// </summary>
        private bool WriteFileHook(
            IntPtr hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped)
        {
            // Call original first so we get lpNumberOfBytesWritten
            var result = Native.WriteFile(hFile, lpBuffer, nNumberOfBytesToWrite, out lpNumberOfBytesWritten, lpOverlapped);

            try
            {
                lock (messageQueue)
                {
                    if (messageQueue.Count < 1000)
                    {
                        // Retrieve filename from the file handle
                        var filename = new StringBuilder(255);
                        Native.GetFinalPathNameByHandle(hFile, filename, 255, 0);

                        // Add message to send to the main program
                        messageQueue.Enqueue($"[{RemoteHooking.GetCurrentProcessId()}:{RemoteHooking.GetCurrentThreadId()}]: WRITE ({lpNumberOfBytesWritten} bytes) \"{filename}\"");
                    }
                }
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            return result;
        }

        #endregion
    }
}