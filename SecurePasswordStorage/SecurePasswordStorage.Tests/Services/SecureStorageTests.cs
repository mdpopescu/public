using System.Linq;
using System.Security;
using System.Text;
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
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(null);
                var secret = ObjectMother.CreateBytes();

                sut.Save(credentials, secret);
            }

            [TestMethod("Throws an exception if the user with the given credentials has an incorrect password")]
            [ExpectedException(typeof(SecurityException))]
            public void Test2B()
            {
                var credentials = ObjectMother.CreateCredentials();
                var user = ObjectMother.CreateUser(credentials.Key);
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(user);
                // hashing the password returns another value than the one stored in the user record
                A.CallTo(() => crypto.SecureHash(user.Salt.Concat(Encoding.UTF8.GetBytes(credentials.Password)).ToArray())).Returns(ObjectMother.CreateBytes());
                var secret = ObjectMother.CreateBytes();

                sut.Save(credentials, secret);
            }

            [TestMethod("Transforms the credentials")]
            public void Test3()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                var secret = ObjectMother.CreateBytes();

                sut.Save(credentials, secret);

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

        [TestClass]
        public class Load : SecureStorageTests
        {
            [TestMethod("Ensures that the user credentials are first verified in the user repository")]
            public void Test1()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                SetupValidSecret(credentials, A.Dummy<byte[]>(), out _);

                sut.Load(credentials);

                A.CallTo(() => userRepository.Load(credentials.Key)).MustHaveHappened();
            }

            [TestMethod("Throws an exception if the user with the given credentials cannot be found")]
            [ExpectedException(typeof(SecurityException))]
            public void Test2A()
            {
                var credentials = ObjectMother.CreateCredentials();
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(null);

                sut.Load(credentials);
            }

            [TestMethod("Throws an exception if the user with the given credentials has an incorrect password")]
            [ExpectedException(typeof(SecurityException))]
            public void Test2B()
            {
                var credentials = ObjectMother.CreateCredentials();
                var user = ObjectMother.CreateUser(credentials.Key);
                A.CallTo(() => userRepository.Load(credentials.Key)).Returns(user);
                // hashing the password returns another value than the one stored in the user record
                A.CallTo(() => crypto.SecureHash(user.Salt.Concat(Encoding.UTF8.GetBytes(credentials.Password)).ToArray())).Returns(ObjectMother.CreateBytes());

                sut.Load(credentials);
            }

            [TestMethod("Transforms the credentials")]
            public void Test3()
            {
                var credentials = ObjectMother.CreateCredentials();
                SetupValidUser(credentials);
                SetupValidSecret(credentials, A.Dummy<byte[]>(), out _);

                sut.Load(credentials);

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

                sut.Load(credentials);

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

                sut.Load(credentials);
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

                sut.Load(credentials);

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

                var result = sut.Load(credentials);

                CollectionAssert.AreEqual(secret, result);
            }
        }

        //

        private void SetupValidUser(Credentials credentials)
        {
            var user = ObjectMother.CreateUser(credentials.Key);
            A.CallTo(() => userRepository.Load(credentials.Key)).Returns(user);
            // hashing the credentials returns the hash stored in the user record
            A
                .CallTo(
                    () => crypto.SecureHash(
                        A<byte[]>.That.Matches(bytes => bytes.SequenceEqual(user.Salt.Concat(Encoding.UTF8.GetBytes(credentials.Password)).ToArray()))))
                .Returns(user.PasswordHash);
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