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
                .Subscribe(m => channel.Send(new Message(m.AccountId, interpreter.Execute(m.AccountId, m.Text))));
        }

        //

        private readonly Executable interpreter;
    }
}