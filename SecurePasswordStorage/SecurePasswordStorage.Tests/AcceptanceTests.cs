using System;
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
        [TestMethod("The application stores the 3rd party credentials")]
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

            var pl = ObjectMother.CreateBytes();
            var pr = ObjectMother.CreateBytes();
            A.CallTo(() => crypto.LargeHash(credentials.Password)).Returns(Tuple.Create(pl, pr));

            var secureKey = ObjectMother.CreateBytes();
            A.CallTo(() => crypto.SecureHash(pl)).Returns(secureKey);

            var encryptedSecret = ObjectMother.CreateBytes();
            A.CallTo(() => crypto.Encrypt(secureKey, secret)).Returns(encryptedSecret);

            var verificationHash = ObjectMother.CreateBytes();
            A.CallTo(() => crypto.SecureHash(pr)).Returns(verificationHash);

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
    }
}