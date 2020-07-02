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
            // composition root
            var fs = new Mock<IFileSystem>();
            var codeGenerator = new CodeGenerator(
                new Parser(),
                new MemberGenerator(),
                new ClassNameGenerator()
            );
            var filenameGenerator = new FilenameGenerator();
            var app = new App(fs.Object, codeGenerator, filenameGenerator);

            // set up the mocks
            const string CODE = @"interface ITest
{
    int A { get; }
    string B { get; set; }

    void SomeMethod(int a, string b);
    int GetStuff(bool flag);
}";
            fs.Setup(it => it.ReadText("ITest.cs")).Returns(CODE);

            // invoke the generator
            app.GenerateDecorator("ITest.cs");

            // verify the result
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