using System;
using System.Linq;
using System.Security;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurePasswordStorage.Library.Contracts;
using SecurePasswordStorage.Library.Models;
using SecurePasswordStorage.Library.Services;

namespace SecurePasswordStorage.Tests.Services
{
    [TestClass]
    public class SecureStorageTests
    {
        private ICryptoFacade crypto;
        private IUserRepository userRepository;
        private ISecretRepository secretRepository;

        private SecureStorage sut;

        [TestInitialize]
        public void SetUp()
        {
            crypto = A.Fake<ICryptoFacade>();
            userRepository = A.Fake<IUserRepository>();
            secretRepository = A.Fake<ISecretRepository>();

            sut = new SecureStorage(crypto, userRepository, secretRepository);
        }

        [TestClass]
        public class Save : SecureStorageTests
        {
            [TestMethod("Ensures that the user credentials are first verified in the user repository")]
            public void Test1()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                var secret = ObjectMother.CreateBytes();

                sut.Save(credentials, secret);

                A.CallTo(() => userRepository.Load(credentials.Key)).MustHaveHappened();
            }

            [TestMethod("Throws an exception if the user with the given credentials cannot be found")]
            [ExpectedException(typeof(SecurityException))]
            public void Test2A()
            {
                var credentials = ObjectMother.CreateCredentials();
                var secret = ObjectMother.CreateBytes();
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(null);

                sut.Save(credentials, secret);
            }

            [TestMethod("Throws an exception if the user with the given credentials has an incorrect password")]
            [ExpectedException(typeof(SecurityException))]
            public void Test2B()
            {
                var credentials = ObjectMother.CreateCredentials();
                var secret = ObjectMother.CreateBytes();
                var passwordHash = ObjectMother.CreateBytes();
                var user = new User(credentials.Key, passwordHash);
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(user);
                // hashing the password returns another value than the one stored in the user record
                A.CallTo(() => crypto.SecureHash(credentials.Password)).Returns(ObjectMother.CreateBytes());

                sut.Save(credentials, secret);
            }

            [TestMethod("Hashes the user password with a large hash")]
            public void Test3()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                var secret = ObjectMother.CreateBytes();

                sut.Save(credentials, secret);

                A.CallTo(() => crypto.LargeHash(credentials.Password)).MustHaveHappened();
            }

            [TestMethod("Hashes the first part of the large hash securely")]
            public void Test4()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                var secret = ObjectMother.CreateBytes();
                var pl = ObjectMother.CreateBytes();
                var pr = ObjectMother.CreateBytes();
                A.CallTo(() => crypto.LargeHash(credentials.Password)).Returns(Tuple.Create(pl, pr));

                sut.Save(credentials, secret);

                A.CallTo(() => crypto.SecureHash(pl)).MustHaveHappened();
            }

            [TestMethod("Hashes the second part of the large hash securely")]
            public void Test5()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                var secret = ObjectMother.CreateBytes();
                var pl = ObjectMother.CreateBytes();
                var pr = ObjectMother.CreateBytes();
                A.CallTo(() => crypto.LargeHash(credentials.Password)).Returns(Tuple.Create(pl, pr));

                sut.Save(credentials, secret);

                A.CallTo(() => crypto.SecureHash(pr)).MustHaveHappened();
            }

            [TestMethod("Encrypts the secret with the key derived from the first secure hash")]
            public void Test6()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                var secret = ObjectMother.CreateBytes();
                var pl = ObjectMother.CreateBytes();
                var pr = ObjectMother.CreateBytes();
                A.CallTo(() => crypto.LargeHash(credentials.Password)).Returns(Tuple.Create(pl, pr));
                var secureKey = ObjectMother.CreateBytes();
                A.CallTo(() => crypto.SecureHash(pl)).Returns(secureKey);

                sut.Save(credentials, secret);

                A.CallTo(() => crypto.Encrypt(secureKey, secret)).MustHaveHappened();
            }

            [TestMethod("Stores the encrypted secret together with the verification hash")]
            public void Test7()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                var secret = ObjectMother.CreateBytes();
                var pl = ObjectMother.CreateBytes();
                var pr = ObjectMother.CreateBytes();
                A.CallTo(() => crypto.LargeHash(credentials.Password)).Returns(Tuple.Create(pl, pr));
                var secureKey = ObjectMother.CreateBytes();
                A.CallTo(() => crypto.SecureHash(pl)).Returns(secureKey);
                var encryptedSecret = ObjectMother.CreateBytes();
                A.CallTo(() => crypto.Encrypt(secureKey, secret)).Returns(encryptedSecret);
                var verificationHash = ObjectMother.CreateBytes();
                A.CallTo(() => crypto.SecureHash(pr)).Returns(verificationHash);

                sut.Save(credentials, secret);

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

        //

        private void SetupValidUser(Credentials credentials)
        {
            var passwordHash = ObjectMother.CreateBytes();
            var user = new User(credentials.Key, passwordHash);
            A.CallTo(() => userRepository.Load(credentials.Key)).Returns(user);
            // hashing the password returns the hash stored in the user record
            A.CallTo(() => crypto.SecureHash(credentials.Password)).Returns(passwordHash);
        }
    }
}