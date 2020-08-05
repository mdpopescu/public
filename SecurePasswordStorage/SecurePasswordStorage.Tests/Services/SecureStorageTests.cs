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
        private ICrypto crypto;
        private IRepository repository;

        private SecureStorage sut;

        [TestInitialize]
        public void SetUp()
        {
            crypto = A.Fake<ICrypto>();
            repository = A.Fake<IRepository>();

            sut = new SecureStorage(crypto, repository);
        }

        [TestClass]
        public class Save : SecureStorageTests
        {
            [TestMethod("Hashes the user credentials (securely)")]
            public void Test1()
            {
                var loginCredentials = ObjectMother.CreateCredentials();
                var foreignCredentials = ObjectMother.CreateCredentials();

                sut.Save(loginCredentials, foreignCredentials);

                A.CallTo(() => crypto.GetSecureHash(loginCredentials)).MustHaveHappened();
            }

            [TestMethod("Checks the repository for the (hashed) user credentials")]
            public void Test2()
            {
                var loginCredentials = ObjectMother.CreateCredentials();
                var secureHash = ObjectMother.CreateSecureHash();
                A.CallTo(() => crypto.GetSecureHash(loginCredentials)).Returns(secureHash);
                var foreignCredentials = ObjectMother.CreateCredentials();

                sut.Save(loginCredentials, foreignCredentials);

                A.CallTo(() => repository.LoadUser(loginCredentials.Username, secureHash)).MustHaveHappened();
            }

            [TestMethod("If the user checks out, hashes the user credentials with the large hash")]
            public void Test3()
            {
                var loginCredentials = ObjectMother.CreateCredentials();
                var secureHash = ObjectMother.CreateSecureHash();
                A.CallTo(() => crypto.GetSecureHash(loginCredentials)).Returns(secureHash);
                var user = A.Fake<IUser>();
                A.CallTo(() => repository.LoadUser(loginCredentials.Username, secureHash)).Returns(user);
                var foreignCredentials = ObjectMother.CreateCredentials();

                sut.Save(loginCredentials, foreignCredentials);

                A.CallTo(() => crypto.GetLargeHash(loginCredentials)).MustHaveHappened();
            }

            [TestMethod("If the user does not check out, the large hash is not used")]
            public void Test4()
            {
                var loginCredentials = ObjectMother.CreateCredentials();
                var secureHash = ObjectMother.CreateSecureHash();
                A.CallTo(() => crypto.GetSecureHash(loginCredentials)).Returns(secureHash);
                A.CallTo(() => repository.LoadUser(loginCredentials.Username, secureHash)).Returns(null);
                var foreignCredentials = ObjectMother.CreateCredentials();

                sut.Save(loginCredentials, foreignCredentials);

                A.CallTo(() => crypto.GetLargeHash(loginCredentials)).MustNotHaveHappened();
            }

            [TestMethod("Encrypts the foreign credentials using the first part of the large hash as the key")]
            public void Test5()
            {
                var loginCredentials = ObjectMother.CreateCredentials();
                var secureHash = ObjectMother.CreateSecureHash();
                A.CallTo(() => crypto.GetSecureHash(loginCredentials)).Returns(secureHash);
                var user = A.Fake<IUser>();
                A.CallTo(() => repository.LoadUser(loginCredentials.Username, secureHash)).Returns(user);
                var largeHash = ObjectMother.CreateLargeHash();
                A.CallTo(() => crypto.GetLargeHash(loginCredentials)).Returns(largeHash);
                var foreignCredentials = ObjectMother.CreateCredentials();

                sut.Save(loginCredentials, foreignCredentials);

                A.CallTo(() => crypto.Encrypt(largeHash.PartOne, foreignCredentials)).MustHaveHappened();
            }

            [TestMethod("Hashes the second part of the large hash (securely)")]
            public void Test6()
            {
                var loginCredentials = ObjectMother.CreateCredentials();
                var secureHash = ObjectMother.CreateSecureHash();
                A.CallTo(() => crypto.GetSecureHash(loginCredentials)).Returns(secureHash);
                var user = A.Fake<IUser>();
                A.CallTo(() => repository.LoadUser(loginCredentials.Username, secureHash)).Returns(user);
                var largeHash = ObjectMother.CreateLargeHash();
                A.CallTo(() => crypto.GetLargeHash(loginCredentials)).Returns(largeHash);
                var foreignCredentials = ObjectMother.CreateCredentials();

                sut.Save(loginCredentials, foreignCredentials);

                A.CallTo(() => crypto.GetSecureHash(largeHash.PartTwo)).MustHaveHappened();
            }

            [TestMethod("Saves the result to the repository")]
            public void Test7()
            {
                var loginCredentials = ObjectMother.CreateCredentials();
                var secureHash = ObjectMother.CreateSecureHash();
                A.CallTo(() => crypto.GetSecureHash(loginCredentials)).Returns(secureHash);
                var user = A.Fake<IUser>();
                A.CallTo(() => repository.LoadUser(loginCredentials.Username, secureHash)).Returns(user);
                var largeHash = ObjectMother.CreateLargeHash();
                A.CallTo(() => crypto.GetLargeHash(loginCredentials)).Returns(largeHash);
                var foreignCredentials = ObjectMother.CreateCredentials();
                var encryptedCredentials = ObjectMother.CreateEncryptedCredentials();
                A.CallTo(() => crypto.Encrypt(largeHash.PartOne, foreignCredentials)).Returns(encryptedCredentials.Encrypted);
                A.CallTo(() => crypto.GetSecureHash(largeHash.PartTwo)).Returns(encryptedCredentials.Hashed);

                sut.Save(loginCredentials, foreignCredentials);

                A
                    .CallTo(
                        () => repository.SaveEncryptedCredentials(
                            loginCredentials.Username,
                            A<EncryptedCredentials>.That.Matches(
                                it => it.Encrypted == encryptedCredentials.Encrypted && it.Hashed == encryptedCredentials.Hashed
                            )
                        )
                    )
                    .MustHaveHappened();
            }
        }
    }
}