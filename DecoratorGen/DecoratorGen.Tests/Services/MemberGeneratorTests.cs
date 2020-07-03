using AutoBogus;
using DecoratorGen.Library.Models;
using DecoratorGen.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DecoratorGen.Tests.Services
{
    [TestClass]
    public class MemberGeneratorTests
    {
        private MemberGenerator sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new MemberGenerator();
        }

        [TestClass]
        public class Generate : MemberGeneratorTests
        {
            [TestMethod]
            public void ReturnsTheResultOfMemberToString()
            {
                var member = new Mock<Member>();
                var generated = AutoFaker.Generate<string>();
                member.Setup(it => it.ToString()).Returns(generated);

                var result = sut.Generate(member.Object);

                Assert.AreEqual(generated, result);
            }
        }
    }
}