using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            ISecureStorage storage = new SecureStorage(crypto, userRepository, secretRepository);

            // these are the user's credentials for *our* application
            var credentials = ObjectMother.CreateCredentials();

            // this would be the result of serializing the actual secret we want stored securely
            var secret = ObjectMother.CreateBytes();

            SetupValidUser(userRepository, crypto, credentials);

            #region Security preparations

            byte[] _1, _2;
            var secretKey = ObjectMother.CreateBytes();
            var verificationHash = ObjectMother.CreateBytes();
            A.CallTo(() => crypto.Transform(credentials, out _1, out _2)).AssignsOutAndRefParameters(secretKey, verificationHash);

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
            ISecureStorage storage = new SecureStorage(crypto, userRepository, secretRepository);

            // these are the user's credentials for *our* application
            var credentials = ObjectMother.CreateCredentials();

            // this would be the result of serializing the actual secret we want stored securely
            var secret = ObjectMother.CreateBytes();

            SetupValidUser(userRepository, crypto, credentials);

            #region Security preparations

            byte[] _1, _2;
            var secretKey = ObjectMother.CreateBytes();
            var verificationHash = ObjectMother.CreateBytes();
            A.CallTo(() => crypto.Transform(credentials, out _1, out _2)).AssignsOutAndRefParameters(secretKey, verificationHash);

            var encryptedSecret = ObjectMother.CreateBytes();
            A.CallTo(() => crypto.Decrypt(secretKey, encryptedSecret)).Returns(secret);

            var secretData = new SecretData(credentials.Key, encryptedSecret, verificationHash);
            A.CallTo(() => secretRepository.Load(credentials.Key)).Returns(secretData);

            #endregion

            var storedSecret = storage.Load(credentials);

            CollectionAssert.AreEqual(secret, storedSecret);
        }

        //

        private static void SetupValidUser(IUserRepository userRepository, ICryptoFacade crypto, Credentials credentials)
        {
            var user = ObjectMother.CreateUser(credentials.Key);
            A.CallTo(() => userRepository.Load(credentials.Key)).Returns(user);

            // hashing the credentials returns the hash stored in the user record
            A
                .CallTo(() => crypto.SecureHash(A<byte[]>.That.Matches(bytes => bytes.SequenceEqual(GetSaltedCredentials(user, credentials)))))
                .Returns(user.PasswordHash);
        }

        private static IEnumerable<byte> GetSaltedCredentials(User user, Credentials credentials) =>
            user.Salt
                .Concat(Encoding.UTF8.GetBytes(credentials.Key.Value))
                .Concat(Encoding.UTF8.GetBytes(credentials.Password))
                .ToArray();
    }
}