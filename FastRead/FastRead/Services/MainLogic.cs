using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using FastRead.Contracts;
using FastRead.Models;

namespace FastRead.Services
{
    public class MainLogic
    {
        public MainLogic(MainUI ui, KeyParser parser)
        {
            this.ui = ui;
            this.parser = parser;

            commandHandlers = new Dictionary<Command, Action>
            {
                { Command.Demo, Demo },
                { Command.EnterFullScreen, ui.EnterFullScreen },
                { Command.LeaveFullScreen, ui.LeaveFullScreen },
            };
        }

        public void Handle(Keys keys)
        {
            var command = parser.GetCommand(keys);
            if (command != Command.None)
                commandHandlers[command].Invoke();
        }

        public void Display(string text)
        {
            var words = Regex.Matches(text, "[^\\s]+");
            foreach (Match word in words)
            {
                ui.Display(word.Value);
                Thread.Sleep(60000 / WPM);
            }
        }

        //

        private const int WPM = 400;

        private const string DEMO_TEXT =
            @"My name is Taylor Hebert. I am your average high school teen. Well maybe not so average. I go to Winslow, the worst high school in town, and my ex-best friend has been running a two year long bullying campaign to grind me into the dirt. I know I shouldn’t sound so chipper about that, but I can’t help it. The night must be keeping me positive, it’s hard to focus on bad things when your life is about to change. The bad is where it all started though, so I should started back there, a week ago. Everything has to start somewhere, even if it starts in someplace dark and foul.

Let’s just say I hate Winslow. I hate it with a passion. The school is riddled with gangs and cliques, each marking physical and social territory that you cross at your own peril. The faculty just, well, allows it. Unless a gang fight breaks out, they let the gangs basically do whatever. It’s only a black mark on their record if an incident is reported I guess.

I was special though. I had my own personal clique dedicated to harassing me, the Three Horsemen. Madison is the least of them. She is the group spotter. She finds me, and makes sure nobody important is watching while the other two do whatever. Sophia is the physical one. I can always count on her to push me down stairs or kick me in the knee in passing. She seems to really enjoy hurting people just for fun. She isn’t the worst though.";

        private readonly MainUI ui;
        private readonly KeyParser parser;

        private readonly Dictionary<Command, Action> commandHandlers;

        private void Demo()
        {
            Display(DEMO_TEXT);
        }
    }
}