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

            [TestMethod]
            public void ExtractsMethodsWithArguments()
            {
                const string FRAGMENT = "int Method(string a, int b, Person person)";

                var result = sut.ExtractMembers(FRAGMENT).ToArray();
                Assert.AreEqual(1, result.Length);
                var methodMember = result[0] as MethodMember;
                Assert.IsNotNull(methodMember);
                Assert.AreEqual("int", methodMember.TypeName);
                Assert.AreEqual("Method", methodMember.Name);
                Assert.IsNotNull(methodMember.Arguments);
                Assert.AreEqual(3, methodMember.Arguments.Length);
                Assert.AreEqual("string", methodMember.Arguments[0].TypeName);
                Assert.AreEqual("a", methodMember.Arguments[0].Name);
                Assert.AreEqual("int", methodMember.Arguments[1].TypeName);
                Assert.AreEqual("b", methodMember.Arguments[1].Name);
                Assert.AreEqual("Person", methodMember.Arguments[2].TypeName);
                Assert.AreEqual("person", methodMember.Arguments[2].Name);
            }

            [TestMethod]
            public void ExtractsMethodsWithGenericReturnTypesAndArguments()
            {
                const string FRAGMENT = "IEnumerable<int> Method(int a, IEnumerable<string> b)";

                var result = sut.ExtractMembers(FRAGMENT).ToArray();
                Assert.AreEqual(1, result.Length);
                var methodMember = result[0] as MethodMember;
                Assert.IsNotNull(methodMember);
                Assert.AreEqual("IEnumerable<int>", methodMember.TypeName);
                Assert.AreEqual("Method", methodMember.Name);
                Assert.IsNotNull(methodMember.Arguments);
                Assert.AreEqual(2, methodMember.Arguments.Length);
                Assert.AreEqual("int", methodMember.Arguments[0].TypeName);
                Assert.AreEqual("a", methodMember.Arguments[0].Name);
                Assert.AreEqual("IEnumerable<string>", methodMember.Arguments[1].TypeName);
                Assert.AreEqual("b", methodMember.Arguments[1].Name);
            }

            [TestMethod]
            public void ExtractsGenericMethods()
            {
                const string FRAGMENT = "IEnumerable<T> Method<T>(int a, IEnumerable<T> b)";

                var result = sut.ExtractMembers(FRAGMENT).ToArray();
                Assert.AreEqual(1, result.Length);
                var methodMember = result[0] as MethodMember;
                Assert.IsNotNull(methodMember);
                Assert.AreEqual("IEnumerable<T>", methodMember.TypeName);
                Assert.AreEqual("Method<T>", methodMember.Name);
                Assert.IsNotNull(methodMember.Arguments);
                Assert.AreEqual(2, methodMember.Arguments.Length);
                Assert.AreEqual("int", methodMember.Arguments[0].TypeName);
                Assert.AreEqual("a", methodMember.Arguments[0].Name);
                Assert.AreEqual("IEnumerable<T>", methodMember.Arguments[1].TypeName);
                Assert.AreEqual("b", methodMember.Arguments[1].Name);
            }
        }
    }
}