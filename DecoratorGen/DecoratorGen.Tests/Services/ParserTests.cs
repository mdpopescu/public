using System.Linq;
using DecoratorGen.Library.Models;
using DecoratorGen.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DecoratorGen.Tests.Services
{
    [TestClass]
    public class ParserTests
    {
        private Parser sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new Parser();
        }

        [TestClass]
        public class ExtractInterface : ParserTests
        {
            [TestMethod]
            public void ExtractsTheFirstInterfaceFromTheGivenCodeFragment()
            {
                const string FRAGMENT = @"class A
{
    public int B { get; set; }
}

public interface IX
{
    string C { get; set; }
}

internal class D
{
}

public interface IY
{}
";
                var result = sut.ExtractInterface(FRAGMENT);

                Assert.AreEqual("IX", result.Name);
                Assert.AreEqual(
                    @"public interface IX
{
    string C { get; set; }
}",
                    result.Code);
            }
        }

        [TestClass]
        public class ExtractMembers : ParserTests
        {
            [TestMethod]
            public void ExtractsProperties()
            {
                const string FRAGMENT = @"string C { get; set; }";

                var result = sut.ExtractMembers(FRAGMENT).ToArray();

                Assert.AreEqual(1, result.Length);
                var propertyMember = result[0] as PropertyMember;
                Assert.IsNotNull(propertyMember);
                Assert.AreEqual("string", propertyMember.TypeName);
                Assert.AreEqual("C", propertyMember.Name);
                Assert.IsTrue(propertyMember.HasGetter);
                Assert.IsTrue(propertyMember.HasSetter);
            }

            [TestMethod]
            public void ExtractsReadOnlyProperties()
            {
                const string FRAGMENT = @"string C { get; }";

                var result = sut.ExtractMembers(FRAGMENT).ToArray();

                Assert.AreEqual(1, result.Length);
                var propertyMember = result[0] as ReadOnlyPropertyMember;
                Assert.IsNotNull(propertyMember);
                Assert.AreEqual("string", propertyMember.TypeName);
                Assert.AreEqual("C", propertyMember.Name);
                Assert.IsTrue(propertyMember.HasGetter);
                Assert.IsFalse(propertyMember.HasSetter);
            }

            [TestMethod]
            public void ExtractsWriteOnlyProperties()
            {
                const string FRAGMENT = @"string C { set; }";

                var result = sut.ExtractMembers(FRAGMENT).ToArray();

                Assert.AreEqual(1, result.Length);
                var propertyMember = result[0] as PropertyMember;
                Assert.IsNotNull(propertyMember);
                Assert.AreEqual("string", propertyMember.TypeName);
                Assert.AreEqual("C", propertyMember.Name);
                Assert.IsFalse(propertyMember.HasGetter);
                Assert.IsTrue(propertyMember.HasSetter);
            }

            [TestMethod]
            public void ExtractsMethodsWithoutArguments()
            {
                const string FRAGMENT = "void Method()";

                var result = sut.ExtractMembers(FRAGMENT).ToArray();
                Assert.AreEqual(1, result.Length);
                var methodMember = result[0] as MethodMember;
                Assert.IsNotNull(methodMember);
                Assert.AreEqual("void", methodMember.TypeName);
                Assert.AreEqual("Method", methodMember.Name);
                Assert.IsNotNull(methodMember.Arguments);
                Assert.AreEqual(0, methodMember.Arguments.Length);
            }
        }
    }
}