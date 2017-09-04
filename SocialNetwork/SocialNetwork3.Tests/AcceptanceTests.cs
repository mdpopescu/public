using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork3.Library.Services;

namespace SocialNetwork3.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod]
        public void EndToEnd()
        {
            var input = new NetworkCoordinator();

            // posting
            Sys.Time = () => new DateTime(2000, 1, 1, 10, 0, 0);
            input.Enter("Alice -> I love the weather today");

            Sys.Time = () => new DateTime(2000, 1, 1, 10, 3, 0);
            input.Enter("Bob -> Damn! We lost!");

            Sys.Time = () => new DateTime(2000, 1, 1, 10, 4, 0);
            input.Enter("Bob -> Good game though.");

            Sys.Time = () => new DateTime(2000, 1, 1, 10, 5, 0);
            input.Enter("Charlie -> I'm in New York today! Anyone want to have a coffee?");

            // reading
            Sys.Time = () => new DateTime(2000, 1, 1, 10, 5, 0);

            var response1 = input.Enter("Alice").ToList();
            Assert.AreEqual(1, response1.Count);
            Assert.AreEqual("I love the weather today (5 minutes ago)", response1[0]);

            var response2 = input.Enter("Bob").ToList();
            Assert.AreEqual(2, response2.Count);
            Assert.AreEqual("Good game though. (1 minute ago)", response2[0]);
            Assert.AreEqual("Damn! We lost! (2 minutes ago)", response2[1]);

            // following
            Sys.Time = () => new DateTime(2000, 1, 1, 10, 5, 2);

            input.Enter("Charlie follows Alice");
            var response3 = input.Enter("Charlie wall").ToList();
            Assert.AreEqual(2, response3.Count);
            Assert.AreEqual("Charlie - I'm in New York today! Anyone want to have a coffee? (2 seconds ago)", response3[0]);
            Assert.AreEqual("Alice - I love the weather today (5 minutes ago)", response3[1]);

            input.Enter("Charlie follows Bob");
            var response4 = input.Enter("Charlie wall").ToList();
            Assert.AreEqual(4, response4.Count);
            Assert.AreEqual("Charlie - I'm in New York today! Anyone want to have a coffee? (2 seconds ago)", response4[0]);
            Assert.AreEqual("Bob - Good game though. (1 minute ago)", response4[1]);
            Assert.AreEqual("Bob - Damn! We lost! (2 minutes ago)", response4[2]);
            Assert.AreEqual("Alice - I love the weather today (5 minutes ago)", response4[3]);
        }
    }
}
