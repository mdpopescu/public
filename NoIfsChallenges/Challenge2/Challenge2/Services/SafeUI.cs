using System;
using Challenge2.Library.Contracts;

namespace Challenge2.Services
{
    public class SafeUI : UserInterface
    {
        public bool StartStopEnabled
        {
            set => safeInvoke(() => ui.StartStopEnabled = value);
        }

        public bool ResetEnabled
        {
            set => safeInvoke(() => ui.ResetEnabled = value);
        }

        public bool HoldEnabled
        {
            set => safeInvoke(() => ui.HoldEnabled = value);
        }

        public string Display
        {
            set => safeInvoke(() => ui.Display = value);
        }

        public SafeUI(UserInterface ui, Action<Action> safeInvoke)
        {
            this.ui = ui;
            this.safeInvoke = safeInvoke;
        }

        //

        private readonly UserInterface ui;
        private readonly Action<Action> safeInvoke;
    }
}