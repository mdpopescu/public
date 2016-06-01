﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork2.Library.Implementations;

namespace SocialNetwork2.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        private UserRepository userRepository;

        [TestInitialize]
        public void SetUp()
        {
            userRepository = new UserRepository();
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