using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurePasswordStorage.Library.Contracts;
using SecurePasswordStorage.Library.Services;

namespace SecurePasswordStorage.Tests.Services
{
    [TestClass]
    public class SecurityLogicTests
    {
        private ICrypto crypto;

        private SecurityLogic sut;

        [TestInitialize]
        public void SetUp()
        {
            crypto = A.Fake<ICrypto>();

            sut = new SecurityLogic(crypto);
        }

        [TestClass]
        public class GetEncryptedCredentials : SecurityLogicTests
        {
            [TestMethod("Returns the encrypted credentials")]
            public void Test1()
            {
                var loginCredentials = ObjectMother.CreateCredentials();
                var foreignCredentials = ObjectMother.CreateCredentials();
                var largeHash = ObjectMother.CreateLargeHash();
                A.CallTo(() => crypto.GetLargeHash(loginCredentials)).Returns(largeHash);
                var encrypted = ObjectMother.CreateBytes(8);
                A.CallTo(() => crypto.Encrypt(largeHash.PartOne, foreignCredentials)).Returns(encrypted);
                var secureHash = ObjectMother.CreateSecureHash();
                A.CallTo(() => crypto.GetSecureHash(largeHash.PartTwo)).Returns(secureHash);

                var result = sut.GetEncryptedCredentials(loginCredentials, foreignCredentials);

                CollectionAssert.AreEqual(encrypted, result.Encrypted);
                Assert.AreEqual(secureHash, result.Hashed);
            }
        }
    }
}