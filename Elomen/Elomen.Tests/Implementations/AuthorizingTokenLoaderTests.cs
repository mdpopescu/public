using System;
using CoreTweet;
using Elomen.Storage.Contracts;
using Elomen.Storage.Implementations;
using Elomen.TwitterLibrary.Contracts;
using Elomen.TwitterLibrary.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Elomen.Tests.Implementations
{
    [TestClass]
    public class AuthorizingTokenLoaderTests
    {
        private Mock<Loadable<Tokens>> loader;
        private Mock<Loadable<CompositeSettings>> appStore;
        private Mock<ResourceStore<CompositeSettings>> userStore;
        private Mock<Authorizable> authorizer;

        private AuthorizingTokenLoader sut;

        [TestInitialize]
        public void SetUp()
        {
            loader = new Mock<Loadable<Tokens>>();
            appStore = new Mock<Loadable<CompositeSettings>>();
            userStore = new Mock<ResourceStore<CompositeSettings>>();
            authorizer = new Mock<Authorizable>();

            sut = new AuthorizingTokenLoader(loader.Object, appStore.Object, userStore.Object, authorizer.Object);
        }

        [TestClass]
        public class Load : AuthorizingTokenLoaderTests
        {
            [TestMethod]
            public void ReturnsTheResultOfTheUnderlyingLoad()
            {
                var tokens = new Tokens();
                loader.Setup(it => it.Load()).Returns(tokens);

                var result = sut.Load();

                Assert.AreEqual(tokens, result);
            }

            [TestMethod]
            public void ReturnsTheResultOfAuthorizationIfTheUnderlyingLoadThrows()
            {
                loader.Setup(it => it.Load()).Throws<Exception>();
                var appSettings = new DictionarySettings
                {
                    ["ConsumerKey"] = "a",
                    ["ConsumerSecret"] = "b",
                };
                appStore.Setup(it => it.Load()).Returns(appSettings);
                var tokens = new Tokens();
                authorizer.Setup(it => it.Authorize("a", "b")).Returns(tokens);
                userStore.Setup(it => it.Load()).Returns(new DictionarySettings());

                var result = sut.Load();

                Assert.AreEqual(tokens, result);
            }

            [TestMethod]
            public void SavesTheResultOfAuthorizationIfTheUnderlyingLoadThrows()
            {
                loader.Setup(it => it.Load()).Throws<Exception>();
                appStore.Setup(it => it.Load()).Returns(new DictionarySettings());
                var tokens = new Tokens
                {
                    AccessToken = "a",
                    AccessTokenSecret = "b",
                };
                authorizer.Setup(it => it.Authorize(It.IsAny<string>(), It.IsAny<string>())).Returns(tokens);
                var userSettings = new DictionarySettings();
                userStore.Setup(it => it.Load()).Returns(userSettings);

                sut.Load();

                userStore.Verify(it => it.Save(userSettings));
                Assert.AreEqual("a", userSettings["AccessToken"]);
                Assert.AreEqual("b", userSettings["AccessTokenSecret"]);
            }
        }
    }
}