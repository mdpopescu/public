using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork2.Library.Implementations;
using SocialNetwork2.Library.Implementations.Handlers;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        private UserRepository userRepository;

        [TestInitialize]
        public void SetUp()
        {
            userRepository = new UserRepository(name => new User(name));
        }

        [TestMethod]
        public void Scenario2_Reading()
        {
            Scenario1_Posting();

            Sys.Time = () => new DateTime(2000, 1, 1, 10, 5, 0);

            var user1 = userRepository.CreateOrFind("Alice");
            var response1 = user1.Read().ToList();
            Assert.AreEqual(1, response1.Count);
            Assert.AreEqual("I love the weather today (5 minutes ago)", response1[0]);

            var user2 = userRepository.CreateOrFind("Bob");
            var response2 = user2.Read().ToList();
            Assert.AreEqual(2, response2.Count);
            Assert.AreEqual("Good game though. (1 minute ago)", response2[0]);
            Assert.AreEqual("Damn! We lost! (2 minutes ago)", response2[1]);
        }

        [TestMethod]
        public void Scenario3_Following()
        {
            Scenario1_Posting();

            Sys.Time = () => new DateTime(2000, 1, 1, 10, 5, 0);

            var user = userRepository.CreateOrFind("Charlie");
            user.Post("I'm in New York today! Anyone want to have a coffee?");

            Sys.Time = () => new DateTime(2000, 1, 1, 10, 5, 2);

            user.Follow(userRepository.CreateOrFind("Alice"));
            var response1 = user.Wall().ToList();
            Assert.AreEqual(2, response1.Count);
            Assert.AreEqual("Charlie - I'm in New York today! Anyone want to have a coffee? (2 seconds ago)", response1[0]);
            Assert.AreEqual("Alice - I love the weather today (5 minutes ago)", response1[1]);

            user.Follow(userRepository.CreateOrFind("Bob"));
            var response2 = user.Wall().ToList();
            Assert.AreEqual(4, response2.Count);
            Assert.AreEqual("Charlie - I'm in New York today! Anyone want to have a coffee? (2 seconds ago)", response2[0]);
            Assert.AreEqual("Bob - Good game though. (1 minute ago)", response2[1]);
            Assert.AreEqual("Bob - Damn! We lost! (2 minutes ago)", response2[2]);
            Assert.AreEqual("Alice - I love the weather today (5 minutes ago)", response2[3]);
        }

        [TestMethod]
        public void UsingTheInputHandler()
        {
            var knownCommands = new List<IHandler>
            {
                new ReadHandler(),
                new PostHandler(),
                new FollowsHandler(userRepository),
                new WallHandler(),
            };
            var inputHandler = new InputHandler(userRepository, knownCommands);

            // posting
            Sys.Time = () => new DateTime(2000, 1, 1, 10, 0, 0);
            inputHandler.Handle("Alice -> I love the weather today");

            Sys.Time = () => new DateTime(2000, 1, 1, 10, 3, 0);
            inputHandler.Handle("Bob -> Damn! We lost!");

            Sys.Time = () => new DateTime(2000, 1, 1, 10, 4, 0);
            inputHandler.Handle("Bob -> Good game though.");

            // reading
            Sys.Time = () => new DateTime(2000, 1, 1, 10, 5, 0);

            var response1 = inputHandler.Handle("Alice").ToList();
            Assert.AreEqual(1, response1.Count);
            Assert.AreEqual("I love the weather today (5 minutes ago)", response1[0]);

            var response2 = inputHandler.Handle("Bob").ToList();
            Assert.AreEqual(2, response2.Count);
            Assert.AreEqual("Good game though. (1 minute ago)", response2[0]);
            Assert.AreEqual("Damn! We lost! (2 minutes ago)", response2[1]);

            // following
            Sys.Time = () => new DateTime(2000, 1, 1, 10, 5, 0);
            inputHandler.Handle("Charlie -> I'm in New York today! Anyone want to have a coffee?");

            Sys.Time = () => new DateTime(2000, 1, 1, 10, 5, 2);

            inputHandler.Handle("Charlie follows Alice");
            var response3 = inputHandler.Handle("Charlie wall").ToList();
            Assert.AreEqual(2, response3.Count);
            Assert.AreEqual("Charlie - I'm in New York today! Anyone want to have a coffee? (2 seconds ago)", response3[0]);
            Assert.AreEqual("Alice - I love the weather today (5 minutes ago)", response3[1]);

            inputHandler.Handle("Charlie follows Bob");
            var response4 = inputHandler.Handle("Charlie wall").ToList();
            Assert.AreEqual(4, response4.Count);
            Assert.AreEqual("Charlie - I'm in New York today! Anyone want to have a coffee? (2 seconds ago)", response4[0]);
            Assert.AreEqual("Bob - Good game though. (1 minute ago)", response4[1]);
            Assert.AreEqual("Bob - Damn! We lost! (2 minutes ago)", response4[2]);
            Assert.AreEqual("Alice - I love the weather today (5 minutes ago)", response4[3]);
        }

        //

        private void Scenario1_Posting()
        {
            var user1 = userRepository.CreateOrFind("Alice");
            var user2 = userRepository.CreateOrFind("Bob");

            Sys.Time = () => new DateTime(2000, 1, 1, 10, 0, 0);
            user1.Post("I love the weather today");

            Sys.Time = () => new DateTime(2000, 1, 1, 10, 3, 0);
            user2.Post("Damn! We lost!");

            Sys.Time = () => new DateTime(2000, 1, 1, 10, 4, 0);
            user2.Post("Good game though.");
        }
    }
}