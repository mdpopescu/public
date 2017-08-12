using Microsoft.VisualStudio.TestTools.UnitTesting;
using StaticBlog.Library.Implementations;

namespace StaticBlog.Tests.Implementations
{
    [TestClass]
    public class AuthenticatorTests
    {
        private Authenticator sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new Authenticator();
        }
    }
}