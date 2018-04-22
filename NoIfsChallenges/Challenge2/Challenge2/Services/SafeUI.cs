using System;
using Challenge2.Library.Contracts;

namespace Challenge2.Services
{
    public class SafeUI : UserInterface
    {
        public SafeUI(UserInterface ui, Action<Action> safeInvoke)
        {
            this.ui = ui;
            this.safeInvoke = safeInvoke;
        }

        public void EnableStartStop() => safeInvoke(() => ui.EnableStartStop());
        public void DisableStartStop() => safeInvoke(() => ui.DisableStartStop());

        public void EnableReset() => safeInvoke(() => ui.EnableReset());
        public void DisableReset() => safeInvoke(() => ui.DisableReset());

        public void EnableHold() => safeInvoke(() => ui.EnableHold());
        public void DisableHold() => safeInvoke(() => ui.DisableHold());

        public void Display(string text) => safeInvoke(() => ui.Display(text));

        //

        private readonly UserInterface ui;
        private readonly Action<Action> safeInvoke;
    }
}