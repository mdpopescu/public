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
        private ISecurityLogic securityLogic;
        private IRepository repository;

        private SecureStorage sut;

        [TestInitialize]
        public void SetUp()
        {
            crypto = A.Fake<ICrypto>();
            securityLogic = A.Fake<ISecurityLogic>();
            repository = A.Fake<IRepository>();

            sut = new SecureStorage(crypto, securityLogic, repository);
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

            [TestMethod("If the user checks out, gets the encrypted credentials")]
            public void Test3()
            {
                var loginCredentials = ObjectMother.CreateCredentials();
                var secureHash = ObjectMother.CreateSecureHash();
                A.CallTo(() => crypto.GetSecureHash(loginCredentials)).Returns(secureHash);
                var user = A.Fake<IUser>();
                A.CallTo(() => repository.LoadUser(loginCredentials.Username, secureHash)).Returns(user);
                var foreignCredentials = ObjectMother.CreateCredentials();

                sut.Save(loginCredentials, foreignCredentials);

                A.CallTo(() => securityLogic.GetEncryptedCredentials(loginCredentials, foreignCredentials)).MustHaveHappened();
            }

            [TestMethod("If the user does not check out, the method does not request the encrypted credentials")]
            public void Test4()
            {
                var loginCredentials = ObjectMother.CreateCredentials();
                var secureHash = ObjectMother.CreateSecureHash();
                A.CallTo(() => crypto.GetSecureHash(loginCredentials)).Returns(secureHash);
                A.CallTo(() => repository.LoadUser(loginCredentials.Username, secureHash)).Returns(null);
                var foreignCredentials = ObjectMother.CreateCredentials();

                sut.Save(loginCredentials, foreignCredentials);

                A.CallTo(() => securityLogic.GetEncryptedCredentials(loginCredentials, foreignCredentials)).MustNotHaveHappened();
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
                A.CallTo(() => securityLogic.GetEncryptedCredentials(loginCredentials, foreignCredentials)).Returns(encryptedCredentials);

                sut.Save(loginCredentials, foreignCredentials);

                A
                    .CallTo(
                        () => repository.SaveEncryptedCredentials(
                            loginCredentials.Username,
                            A<EncryptedCredentials>.That.Matches(
                                it => it.Encrypted == encryptedCredentials.Encrypted
                                    && it.Hashed == encryptedCredentials.Hashed
                            )
                        )
                    )
                    .MustHaveHappened();
            }
        }
    }
}