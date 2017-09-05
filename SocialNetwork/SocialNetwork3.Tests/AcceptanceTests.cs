using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork3.Library.Coordinators;
using SocialNetwork3.Library.Logic;

namespace SocialNetwork3.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod]
        public void EndToEnd()
        {
            //var processor = new MessageProcessor();
            var parser = new MessageParser();
            var commandFactory = new CommandFactory(new MessageRepository(), new MessageFormatter());
            var currentTime = DateTime.MinValue;

            // I know that I'm accessing a modified closure here; this is intentional
            // ReSharper disable once AccessToModifiedClosure
            var network = new Network(parser, commandFactory, () => currentTime);

            // posting
            currentTime = new DateTime(2000, 1, 1, 10, 0, 0);
            network.Enter("Alice -> I love the weather today");

            currentTime = new DateTime(2000, 1, 1, 10, 3, 0);
            network.Enter("Bob -> Damn! We lost!");

            currentTime = new DateTime(2000, 1, 1, 10, 4, 0);
            network.Enter("Bob -> Good game though.");

            currentTime = new DateTime(2000, 1, 1, 10, 5, 0);
            network.Enter("Charlie -> I'm in New York today! Anyone want to have a coffee?");

            // reading
            currentTime = new DateTime(2000, 1, 1, 10, 5, 0);

            var response1 = network.Enter("Alice").ToList();
            Assert.AreEqual(1, response1.Count);
            Assert.AreEqual("I love the weather today (5 minutes ago)", response1[0]);

            var response2 = network.Enter("Bob").ToList();
            Assert.AreEqual(2, response2.Count);
            Assert.AreEqual("Good game though. (1 minute ago)", response2[0]);
            Assert.AreEqual("Damn! We lost! (2 minutes ago)", response2[1]);

            // following
            currentTime = new DateTime(2000, 1, 1, 10, 5, 2);

            network.Enter("Charlie follows Alice");
            var response3 = network.Enter("Charlie wall").ToList();
            Assert.AreEqual(2, response3.Count);
            Assert.AreEqual("Charlie - I'm in New York today! Anyone want to have a coffee? (2 seconds ago)", response3[0]);
            Assert.AreEqual("Alice - I love the weather today (5 minutes ago)", response3[1]);

            network.Enter("Charlie follows Bob");
            var response4 = network.Enter("Charlie wall").ToList();
            Assert.AreEqual(4, response4.Count);
            Assert.AreEqual("Charlie - I'm in New York today! Anyone want to have a coffee? (2 seconds ago)", response4[0]);
            Assert.AreEqual("Bob - Good game though. (1 minute ago)", response4[1]);
            Assert.AreEqual("Bob - Damn! We lost! (2 minutes ago)", response4[2]);
            Assert.AreEqual("Alice - I love the weather today (5 minutes ago)", response4[3]);
        }
    }
}