﻿using System.Linq;
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
        [TestMethod("The secure storage stores the secret")]
        public void Test1()
        {
            var crypto = A.Fake<ICryptoFacade>();
            var userRepository = A.Fake<IUserRepository>();
            var secretRepository = A.Fake<ISecretRepository>();
            var storage = new SecureStorage(crypto, userRepository, secretRepository);

            // these are the user's credentials for *our* application
            var credentials = ObjectMother.CreateCredentials();

            // this would be the result of serializing the actual secret we want stored securely
            var secret = ObjectMother.CreateBytes();

            var passwordHash = ObjectMother.CreateBytes();
            A.CallTo(() => userRepository.Load(credentials.Key)).Returns(new User(credentials.Key, passwordHash));
            A.CallTo(() => crypto.SecureHash(credentials.Password)).Returns(passwordHash);

            #region Security preparations

            byte[] _1, _2;
            var secretKey = ObjectMother.CreateBytes();
            var verificationHash = ObjectMother.CreateBytes();
            A.CallTo(() => crypto.TransformPassword(credentials.Password, out _1, out _2)).AssignsOutAndRefParameters(secretKey, verificationHash);

            var encryptedSecret = ObjectMother.CreateBytes();
            A.CallTo(() => crypto.Encrypt(secretKey, secret)).Returns(encryptedSecret);

            #endregion

            storage.Save(credentials, secret);

            A
                .CallTo(
                    () => secretRepository.Save(
                        A<SecretData>.That.Matches(
                            data => data.Key == credentials.Key
                                && data.EncryptedSecret.SequenceEqual(encryptedSecret)
                                && data.VerificationHash.SequenceEqual(verificationHash)
                        )
                    )
                )
                .MustHaveHappened();
        }

        [TestMethod("The secure storage retrieves the secret")]
        public void Test2()
        {
            var crypto = A.Fake<ICryptoFacade>();
            var userRepository = A.Fake<IUserRepository>();
            var secretRepository = A.Fake<ISecretRepository>();
            var storage = new SecureStorage(crypto, userRepository, secretRepository);

            // these are the user's credentials for *our* application
            var credentials = ObjectMother.CreateCredentials();

            // this would be the result of serializing the actual secret we want stored securely
            var secret = ObjectMother.CreateBytes();

            var passwordHash = ObjectMother.CreateBytes();
            A.CallTo(() => userRepository.Load(credentials.Key)).Returns(new User(credentials.Key, passwordHash));
            A.CallTo(() => crypto.SecureHash(credentials.Password)).Returns(passwordHash);

            #region Security preparations

            byte[] _1, _2;
            var secretKey = ObjectMother.CreateBytes();
            var verificationHash = ObjectMother.CreateBytes();
            A.CallTo(() => crypto.TransformPassword(credentials.Password, out _1, out _2)).AssignsOutAndRefParameters(secretKey, verificationHash);

            var encryptedSecret = ObjectMother.CreateBytes();
            A.CallTo(() => crypto.Decrypt(secretKey, encryptedSecret)).Returns(secret);

            var secretData = new SecretData(credentials.Key, encryptedSecret, verificationHash);
            A.CallTo(() => secretRepository.Load(credentials.Key)).Returns(secretData);

            #endregion

            var storedSecret = storage.Load(credentials);

            CollectionAssert.AreEqual(secret, storedSecret);
        }
    }
}