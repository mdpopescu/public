using System.Reactive.Subjects;
using Elomen.Library.Contracts;
using Elomen.Library.Implementations;
using Elomen.Library.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Elomen.Tests.Implementations
{
    [TestClass]
    public class ChannelMonitorTests
    {
        private Mock<Executable> interpreter;

        private ChannelMonitor sut;

        [TestInitialize]
        public void SetUp()
        {
            interpreter = new Mock<Executable>();

            sut = new ChannelMonitor(interpreter.Object);
        }

        [TestMethod]
        public void ExecutesIncomingCommands()
        {
            var channel = new Mock<Channel>();
            using (var incoming = new Subject<Message>())
            {
                channel
                    .Setup(it => it.Receive())
                    .Returns(incoming);
                var account = new Account(1, "guest");
                interpreter
                    .Setup(it => it.Execute(account, "command"))
                    .Returns("response");
                sut.Monitor(channel.Object);

                incoming.OnNext(new Message(account, "command"));

                channel.Verify(it => it.Send(It.Is<Message>(m => m.Account == account && m.Text == "response")));
            }
        }
    }
}