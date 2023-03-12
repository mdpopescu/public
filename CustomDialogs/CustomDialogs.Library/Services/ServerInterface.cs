using System;

namespace CustomDialogs.Library.Services
{
    /// <summary>
    ///     Provides an interface for communicating from the client (target) to the server (injector)
    /// </summary>
    public class ServerInterface : MarshalByRefObject
    {
        public void IsInstalled(int clientPID)
        {
            Mailbox.INSTANCE.Add($"The hook has been injected into process {clientPID}.\r\n");
        }

        /// <summary>
        ///     Output messages to the console.
        /// </summary>
        public void ReportMessages(string[] messages)
        {
            Mailbox.INSTANCE.AddRange(messages);
        }

        public void ReportMessage(string message)
        {
            Mailbox.INSTANCE.Add(message);
        }

        /// <summary>
        ///     Report exception
        /// </summary>
        public void ReportException(Exception e)
        {
            Mailbox.INSTANCE.Add("*** ERROR *** The target process has reported an error:\r\n" + e);
        }

        /// <summary>
        ///     Called to confirm that the IPC channel is still open / host application has not closed
        /// </summary>
        public void Ping()
        {
        }
    }
}