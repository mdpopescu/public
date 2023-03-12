using CustomDialogs.Library.Services;
using CustomDialogs.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels.Ipc;
using Telerik.WinControls.UI;

namespace CustomDialogs
{
    public partial class MainForm : RadForm
    {
        public MainForm()
        {
            InitializeComponent();

            injector = new Injector();
        }

        //

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var notepadProcess = Process.GetProcessesByName("Notepad").FirstOrDefault();
            if (notepadProcess != null)
                channel = injector.Run(notepadProcess);
        }

        protected override void OnClosed(EventArgs e)
        {
            channel = null;

            base.OnClosed(e);
        }

        //

        private readonly Injector injector;

        // this is used to keep the reference to the channel alive
        // ReSharper disable once NotAccessedField.Local
        private IpcServerChannel channel;

        //

        private void timer_Tick(object sender, EventArgs e)
        {
            var messages = Mailbox.INSTANCE.Get();
            foreach (var message in messages)
                rtxtLog.Text += message + Environment.NewLine;
        }
    }
}