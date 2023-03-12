using CustomDialogs.Library.Services;
using EasyHook;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;

namespace CustomDialogs.Services
{
    internal class Injector
    {
        public IpcServerChannel Run(Process targetProcess)
        {
            var targetPID = targetProcess.Id;

            // Will contain the name of the IPC server channel
            string channelName = null;

            // Create the IPC server using the ServiceInterface class as a singleton
            var result = RemoteHooking.IpcCreateServer<ServerInterface>(ref channelName, WellKnownObjectMode.Singleton);

            // Get the full path to the assembly we want to inject into the target process
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("Cannot determine the location of this executable.");
            var injectionLibrary = Path.Combine(directoryName, "CustomDialogs.Library.dll");

            try
            {
                // Injecting into existing process by Id
                Mailbox.INSTANCE.Add($"Attempting to inject into process {targetPID}");

                // inject into existing process
                RemoteHooking.Inject(
                    targetPID, // ID of process to inject into
                    injectionLibrary, // 32-bit library to inject (if target is 32-bit)
                    injectionLibrary, // 64-bit library to inject (if target is 64-bit)
                    channelName // the parameters to pass into injected library
                );

                return result;
            }
            catch (Exception e)
            {
                Mailbox.INSTANCE.Add($"*** ERROR *** There was an error while injecting into target:\r\n{e}");
                return null;
            }
        }
    }
}