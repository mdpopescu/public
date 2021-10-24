using System;
using Challenge2New.Library.Models;

namespace Challenge2New.Library.Contracts
{
    public interface IUserInterface
    {
        void SetUp(EventHandler onStartStop, EventHandler onHold, EventHandler onReset);
        void TearDown(EventHandler onStartStop, EventHandler onHold, EventHandler onReset);

        void SetEnabled(OperableActionName actionName, bool value);
        void SetDisplay(string value);
    }
}