using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurePasswordStorage.Library.Contracts;
using SecurePasswordStorage.Library.Services;

namespace SecurePasswordStorage.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod("The application stores the 3rd party credentials")]
        public void Test1()
        {
            var crypto = A.Fake<ICrypto>();
            var securityLogic = new SecurityLogic(crypto);
            var repository = A.Fake<IRepository>();
            ISecureStorage storage = new SecureStorage(crypto, securityLogic, repository);

            var loginCredentials = ObjectMother.CreateCredentials();
            var foreignCredentials = ObjectMother.CreateCredentials();
            var encryptedCredentials = ObjectMother.CreateEncryptedCredentials();

            var secureHash = ObjectMother.CreateSecureHash();
            A.CallTo(() => crypto.GetSecureHash(loginCredentials)).Returns(secureHash);
            var largeHash = ObjectMother.CreateLargeHash();
            A.CallTo(() => crypto.GetLargeHash(loginCredentials)).Returns(largeHash);

            storage.Save(loginCredentials, foreignCredentials);

            A.CallTo(() => repository.SaveEncryptedCredentials(loginCredentials.Username, encryptedCredentials)).MustHaveHappened();
        }
    }
}