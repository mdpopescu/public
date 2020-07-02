using DecoratorGen.Library.Contracts;
using DecoratorGen.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DecoratorGen.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod]
        public void Scenario1()
        {
            var fs = new Mock<IFileSystem>();
            const string CODE = @"interface ITest
{
    int A { get; }
    string B { get; set; }

    void SomeMethod(int a, string b);
    int GetStuff(bool flag);
}";
            fs.Setup(it => it.ReadText("ITest.cs")).Returns(CODE);
            var codeGenerator = new CodeGenerator();
            var filenameGenerator = new FilenameGenerator();
            var app = new App(fs.Object, codeGenerator, filenameGenerator);

            app.GenerateDecorator("ITest.cs");

            fs.Verify(
                it => it.WriteText(
                    "TestDecorator.cs",
                    @"public class TestDecorator
{
    public int A => decorated.A;
    public string B
    {
        get => decorated.B;
        set => decorated.B = value;
    }

    public XWrapper(IX decorated)
    {
        this.decorated = decorated;
    }

    public void SomeMethod(int a, string b) =>
        decorated.SomeMethod(a, b);


    public int GetStuff(bool flag) =>
        decorated.GetStuff(flag);

    //

    private IX decorated;
}"));
        }
    }
}