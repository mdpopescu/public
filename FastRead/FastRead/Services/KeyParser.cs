using System.Windows.Forms;
using FastRead.Models;

namespace FastRead.Services
{
    public class KeyParser
    {
        public Command GetCommand(Keys keys)
        {
            if (keys == Keys.Home)
                return Command.Demo;
            if (keys == (Keys.Alt | Keys.Enter))
                return Command.EnterFullScreen;
            if (keys == Keys.Escape)
                return Command.LeaveFullScreen;

            return Command.None;
        }
    }
}