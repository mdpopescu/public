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
        public class SaveUser : SecureStorageTests
        {
            [TestMethod("Saves a user object based on the given credentials")]
            public void Test1()
            {
                var credentials = ObjectMother.CreateCredentials();
                var salt = ObjectMother.CreateBytes();
                var hash = ObjectMother.CreateBytes();
                A.CallTo(() => crypto.GenerateHash(credentials)).Returns(ByteArrayTuple.Create(salt, hash));

                sut.SaveUser(credentials);

                A.CallTo(
                        () => userRepository.Save(
                            A<User>.That.Matches(
                                it => it.Key == credentials.Key
                                    && it.Salt.SequenceEqual(salt)
                                    && it.PasswordHash.SequenceEqual(hash)
                            )
                        )
                    )
                    .MustHaveHappened();
            }
        }

        [TestClass]
        public class LoadUser : SecureStorageTests
        {
            [TestMethod("Loads the user with the given key")]
            public void Test1()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);

                sut.LoadUser(credentials);

                A.CallTo(() => userRepository.Load(credentials.Key)).MustHaveHappened();
            }

            [TestMethod("Throws an exception if the user with the given key does not exist")]
            [ExpectedException(typeof(SecurityException))]
            public void Test2()
            {
                var credentials = ObjectMother.CreateCredentials();
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(null);

                sut.LoadUser(credentials);
            }

            [TestMethod("Throws an exception if the credentials do not match")]
            [ExpectedException(typeof(SecurityException))]
            public void Test3()
            {
                var credentials = ObjectMother.CreateCredentials();
                var user = ObjectMother.CreateUser(credentials.Key);
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(user);
                A.CallTo(() => crypto.VerifyHash(credentials, user.Salt, user.PasswordHash)).Returns(false);

                sut.LoadUser(credentials);
            }

            [TestMethod("Returns the user")]
            public void Test4()
            {
                var credentials = ObjectMother.CreateCredentials();
                var user = ObjectMother.CreateUser(credentials.Key);
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(user);
                A.CallTo(() => crypto.VerifyHash(credentials, user.Salt, user.PasswordHash)).Returns(true);

                var result = sut.LoadUser(credentials);

                Assert.AreEqual(user, result);
            }
        }

        [TestClass]
        public class CheckUser : SecureStorageTests
        {
            [TestMethod("Loads the user with the given key")]
            public void Test1()
            {
                var credentials = ObjectMother.CreateCredentials();

                sut.CheckUser(credentials);

                A.CallTo(() => userRepository.Load(credentials.Key)).MustHaveHappened();
            }

            [TestMethod("Returns false if the user with the given key does not exist")]
            public void Test2()
            {
                var credentials = ObjectMother.CreateCredentials();
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(null);

                var result = sut.CheckUser(credentials);

                Assert.IsFalse(result);
            }

            [TestMethod("Returns false if the credentials do not match")]
            public void Test3()
            {
                var credentials = ObjectMother.CreateCredentials();
                var user = ObjectMother.CreateUser(credentials.Key);
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(user);
                A.CallTo(() => crypto.VerifyHash(credentials, user.Salt, user.PasswordHash)).Returns(false);

                var result = sut.CheckUser(credentials);

                Assert.IsFalse(result);
            }

            [TestMethod("Returns true if the user exists and the credentials match")]
            public void Test4()
            {
                var credentials = ObjectMother.CreateCredentials();
                var user = ObjectMother.CreateUser(credentials.Key);
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(user);
                A.CallTo(() => crypto.VerifyHash(credentials, user.Salt, user.PasswordHash)).Returns(true);

                var result = sut.CheckUser(credentials);

                Assert.IsTrue(result);
            }
        }

        [TestClass]
        public class SaveSecret : SecureStorageTests
        {
            [TestMethod("Ensures that the user credentials are first verified in the user repository")]
            public void Test1()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                var secret = ObjectMother.CreateBytes();

                sut.SaveSecret(credentials, secret);

                A.CallTo(() => userRepository.Load(credentials.Key)).MustHaveHappened();
            }

            [TestMethod("Throws an exception if the user with the given credentials cannot be found")]
            [ExpectedException(typeof(SecurityException))]
            public void Test2A()
            {
                var credentials = ObjectMother.CreateCredentials();
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(null);
                var secret = ObjectMother.CreateBytes();

                sut.SaveSecret(credentials, secret);
            }

            [TestMethod("Throws an exception if the user with the given credentials has an incorrect password")]
            [ExpectedException(typeof(SecurityException))]
            public void Test2B()
            {
                var credentials = ObjectMother.CreateCredentials();
                var user = ObjectMother.CreateUser(credentials.Key);
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(user);
                A.CallTo(() => crypto.VerifyHash(credentials, A<byte[]>.Ignored, A<byte[]>.Ignored)).Returns(false);
                var secret = ObjectMother.CreateBytes();

                sut.SaveSecret(credentials, secret);
            }

            [TestMethod("Transforms the credentials")]
            public void Test3()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                var secret = ObjectMother.CreateBytes();

                sut.SaveSecret(credentials, secret);

                byte[] secureKey;
                byte[] verificationHash;
                A.CallTo(() => crypto.Transform(credentials, out secureKey, out verificationHash)).MustHaveHappened();
            }

            [TestMethod("Stores the encrypted secret together with the verification hash")]
            public void Test4()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                var secret = ObjectMother.CreateBytes();
                var secureKey = ObjectMother.CreateBytes();
                var encryptedSecret = ObjectMother.CreateBytes();
                var verificationHash = ObjectMother.CreateBytes();
                byte[] _1, _2;
                A.CallTo(() => crypto.Transform(credentials, out _1, out _2)).AssignsOutAndRefParameters(secureKey, verificationHash);
                A.CallTo(() => crypto.Encrypt(secureKey, secret)).Returns(encryptedSecret);

                sut.SaveSecret(credentials, secret);

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

        [TestClass]
        public class LoadSecret : SecureStorageTests
        {
            [TestMethod("Ensures that the user credentials are first verified in the user repository")]
            public void Test1()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                SetupValidSecret(credentials, A.Dummy<byte[]>(), out _);

                sut.LoadSecret(credentials);

                A.CallTo(() => userRepository.Load(credentials.Key)).MustHaveHappened();
            }

            [TestMethod("Throws an exception if the user with the given credentials cannot be found")]
            [ExpectedException(typeof(SecurityException))]
            public void Test2A()
            {
                var credentials = ObjectMother.CreateCredentials();
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(null);

                sut.LoadSecret(credentials);
            }

            [TestMethod("Throws an exception if the user with the given credentials has an incorrect password")]
            [ExpectedException(typeof(SecurityException))]
            public void Test2B()
            {
                var credentials = ObjectMother.CreateCredentials();
                var user = ObjectMother.CreateUser(credentials.Key);
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(user);
                A.CallTo(() => crypto.VerifyHash(credentials, A<byte[]>.Ignored, A<byte[]>.Ignored)).Returns(false);

                sut.LoadSecret(credentials);
            }

            [TestMethod("Transforms the credentials")]
            public void Test3()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                SetupValidSecret(credentials, A.Dummy<byte[]>(), out _);

                sut.LoadSecret(credentials);

                byte[] secureKey;
                byte[] verificationHash;
                A.CallTo(() => crypto.Transform(credentials, out secureKey, out verificationHash)).MustHaveHappened();
            }

            [TestMethod("Loads the secret data from the repository")]
            public void Test4()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                SetupValidSecret(credentials, A.Dummy<byte[]>(), out _);

                sut.LoadSecret(credentials);

                A.CallTo(() => secretRepository.Load(credentials.Key)).MustHaveHappened();
            }

            [TestMethod("Throws an exception if the verification hash does not match")]
            [ExpectedException(typeof(SecurityException))]
            public void Test5()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                SetupValidSecret(credentials, A.Dummy<byte[]>(), out _);
                var secretData = new SecretData(credentials.Key, ObjectMother.CreateBytes(), ObjectMother.CreateBytes());
                A.CallTo(() => secretRepository.Load(credentials.Key)).Returns(secretData);

                sut.LoadSecret(credentials);
            }

            [TestMethod("Decrypts the secret if the verification hash matches")]
            public void Test6()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                var secretKey = ObjectMother.CreateBytes();
                SetupValidSecret(credentials, secretKey, out var verificationHash);
                var encryptedSecret = ObjectMother.CreateBytes();
                var secretData = new SecretData(credentials.Key, encryptedSecret, verificationHash);
                A.CallTo(() => secretRepository.Load(credentials.Key)).Returns(secretData);

                sut.LoadSecret(credentials);

                A.CallTo(() => crypto.Decrypt(secretKey, encryptedSecret)).MustHaveHappened();
            }

            [TestMethod("Returns the decrypted secret")]
            public void Test7()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                var secretKey = ObjectMother.CreateBytes();
                SetupValidSecret(credentials, secretKey, out var verificationHash);
                var encryptedSecret = ObjectMother.CreateBytes();
                var secretData = new SecretData(credentials.Key, encryptedSecret, verificationHash);
                A.CallTo(() => secretRepository.Load(credentials.Key)).Returns(secretData);
                var secret = ObjectMother.CreateBytes();
                A.CallTo(() => crypto.Decrypt(secretKey, encryptedSecret)).Returns(secret);

                var result = sut.LoadSecret(credentials);

                CollectionAssert.AreEqual(secret, result);
            }
        }

        //

        private void SetupValidUser(Credentials credentials)
        {
            var user = ObjectMother.CreateUser(credentials.Key);
            A.CallTo(() => userRepository.Load(credentials.Key)).Returns(user);
            A.CallTo(
                    () => crypto.VerifyHash(
                        credentials,
                        A<byte[]>.That.Matches(bytes => bytes.SequenceEqual(user.Salt)),
                        A<byte[]>.That.Matches(bytes => bytes.SequenceEqual(user.PasswordHash))
                    )
                )
                .Returns(true);
        }

        private void SetupValidSecret(Credentials credentials, byte[] secretKey, out byte[] verificationHash)
        {
            byte[] _1, _2;
            verificationHash = ObjectMother.CreateBytes();
            A.CallTo(() => crypto.Transform(credentials, out _1, out _2)).AssignsOutAndRefParameters(secretKey, verificationHash);
            var encryptedSecret = ObjectMother.CreateBytes();
            var secret = ObjectMother.CreateBytes();
            A.CallTo(() => crypto.Encrypt(secretKey, secret)).Returns(encryptedSecret);

            var secretData = new SecretData(credentials.Key, encryptedSecret, verificationHash);
            A.CallTo(() => secretRepository.Load(credentials.Key)).Returns(secretData);
        }
    }
}