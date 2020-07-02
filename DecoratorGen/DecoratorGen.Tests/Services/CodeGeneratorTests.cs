using AutoBogus;
using DecoratorGen.Library.Contracts;
using DecoratorGen.Library.Models;
using DecoratorGen.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DecoratorGen.Tests.Services
{
    [TestClass]
    public class CodeGeneratorTests
    {
        private Mock<IParser> parser;
        private Mock<IMemberGenerator> memberGenerator;
        private Mock<IClassNameGenerator> classNameGenerator;

        private CodeGenerator sut;

        [TestInitialize]
        public void SetUp()
        {
            parser = new Mock<IParser>();
            memberGenerator = new Mock<IMemberGenerator>();
            classNameGenerator = new Mock<IClassNameGenerator>();

            sut = new CodeGenerator(parser.Object, memberGenerator.Object, classNameGenerator.Object);
        }

        [TestClass]
        public class Generate : CodeGeneratorTests
        {
            [TestMethod]
            public void ConvertsTheInterfaceNameToAClassName()
            {
                var code = AutoFaker.Generate<string>();
                var interfaceName = AutoFaker.Generate<string>();
                var interfaceCode = AutoFaker.Generate<string>();
                var interfaceData = new InterfaceData { Name = interfaceName, Code = interfaceCode };
                parser.Setup(it => it.ExtractInterface(code)).Returns(interfaceData);

                sut.Generate(code);

                classNameGenerator.Verify(it => it.GenerateClassName(interfaceCode));
            }

            [TestMethod]
            public void ExtractsTheMembersFromTheInterface()
            {
                var code = AutoFaker.Generate<string>();
                var interfaceName = AutoFaker.Generate<string>();
                var interfaceCode = AutoFaker.Generate<string>();
                var interfaceData = new InterfaceData { Name = interfaceName, Code = interfaceCode };
                parser.Setup(it => it.ExtractInterface(code)).Returns(interfaceData);

                sut.Generate(code);

                parser.Verify(it => it.ExtractMembers(interfaceCode));
            }

            [TestMethod]
            public void ConvertsEachMember()
            {
                var code = AutoFaker.Generate<string>();
                var interfaceName = AutoFaker.Generate<string>();
                var interfaceCode = AutoFaker.Generate<string>();
                var interfaceData = new InterfaceData { Name = interfaceName, Code = interfaceCode };
                parser.Setup(it => it.ExtractInterface(code)).Returns(interfaceData);
                var members = new[]
                {
                    new Mock<Member>().Object,
                    new Mock<Member>().Object,
                    new Mock<Member>().Object,
                };
                parser.Setup(it => it.ExtractMembers(interfaceCode)).Returns(members);

                sut.Generate(code);

                foreach (var member in members)
                    memberGenerator.Verify(it => it.Generate(member));
            }

            [TestMethod]
            public void ReturnsTheNewClass()
            {
                var code = AutoFaker.Generate<string>();
                const string INTERFACE_NAME = "ISomething";
                const string INTERFACE_CODE = "interface ISomething {}";
                var interfaceData = new InterfaceData { Name = INTERFACE_NAME, Code = INTERFACE_CODE };
                parser.Setup(it => it.ExtractInterface(code)).Returns(interfaceData);
                classNameGenerator.Setup(it => it.GenerateClassName(INTERFACE_CODE)).Returns("SomethingDecorator");
                var members = new[]
                {
                    new Mock<Member>().Object,
                    new Mock<Member>().Object,
                    new Mock<Member>().Object,
                };
                parser.Setup(it => it.ExtractMembers(INTERFACE_CODE)).Returns(members);
                memberGenerator.Setup(it => it.Generate(members[0])).Returns("M0");
                memberGenerator.Setup(it => it.Generate(members[1])).Returns("M1");
                memberGenerator.Setup(it => it.Generate(members[2])).Returns("M2");

                var result = sut.Generate(code);

                Assert.AreEqual(
                    @"public class SomethingDecorator
{
    private ISomething decorated;

    public SomethingDecorator(ISomething decorated)
    {
        this.decorated = decorated;
    }

    M0
    M1
    M2
}",
                    result);
            }
        }
    }
}