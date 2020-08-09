using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurePasswordStorage.Library.Contracts;
using SecurePasswordStorage.Library.Models;
using SecurePasswordStorage.Library.Services;

namespace SecurePasswordStorage.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod("Can store and retrieve a secret")]
        public void Test1()
        {
            var crypto = new CryptoImpl();
            var userRepository = A.Fake<IUserRepository>();
            var secretRepository = new FakeSecretRepository();
            ISecureStorage storage = new SecureStorage(crypto, userRepository, secretRepository);

            // these are the user's credentials for *our* application
            var credentials = ObjectMother.CreateCredentials();

            SetupValidUser(userRepository, crypto, credentials);

            // this would be the result of serializing the actual secret we want stored securely
            var secret = ObjectMother.CreateBytes();

            storage.SaveSecret(credentials, secret);
            var result = storage.LoadSecret(credentials);

            CollectionAssert.AreEqual(secret, result);
        }

        //

        private static void SetupValidUser(IUserRepository userRepository, ICryptoFacade crypto, Credentials credentials)
        {
            var (salt, hash) = crypto.GenerateHash(credentials);
            var user = new User(credentials.Key, salt, hash);
            A.CallTo(() => userRepository.Load(credentials.Key)).Returns(user);
        }

        //

        private class FakeSecretRepository : ISecretRepository
        {
            public SecretData Load(UserKey key) =>
                secrets.Where(it => it.Key == key).FirstOrDefault();

            public void Save(SecretData entity) =>
                secrets.Add(entity);

            //

            private readonly List<SecretData> secrets = new List<SecretData>();
        }
    }
}