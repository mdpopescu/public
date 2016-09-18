using System;
using Elomen.Library.Contracts;
using Elomen.Library.Model;

namespace Elomen.Library.Implementations
{
    public class ChannelMonitor
    {
        public ChannelMonitor(Executable interpreter)
        {
            this.interpreter = interpreter;
        }

        public void Monitor(Channel channel)
        {
            channel
                .Receive()
                .Subscribe(m => Reply(channel, m));
        }

        //

        private readonly Executable interpreter;

        private void Reply(Sender channel, Message m)
        {
            var response = interpreter.Execute(m.Account.Id, m.Text);
            channel.Send(new Message(m.Account, response));
        }
    }
}