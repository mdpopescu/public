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
            var filenameGenerator = new FilenameGenerator();
            var output = new FileOutput(filenameGenerator, fs.Object);
            var codeGenerator = new CodeGenerator(
                new Parser(),
                new MemberGenerator(),
                new ClassNameGenerator()
            );
            var app = new App(fs.Object, codeGenerator, output);

            // set up the mocks
            const string CODE = @"interface ITest
{
    int A { get; }
    string B { get; set; }

    int Test();
    void SomeMethod(int a, string b);
    int GetStuff(bool flag);
}";
            fs.Setup(it => it.ReadText("ITest.cs")).Returns(CODE);

            // invoke the generator
            app.GenerateDecorator("ITest.cs");

            // verify the result
            fs.Verify(
                it => it.WriteText(
                    @"TestDecorator.cs",
                    @"public class TestDecorator : ITest
{
    public TestDecorator(ITest decorated)
    {
        this.decorated = decorated;
    }

    public int A => decorated.A;
    public string B
    {
        get => decorated.B;
        set => decorated.B = value;
    }
    public int Test() =>
        decorated.Test();
    public void SomeMethod(int a, string b) =>
        decorated.SomeMethod(a, b);
    public int GetStuff(bool flag) =>
        decorated.GetStuff(flag);

    //

    private readonly ITest decorated;
}"));
        }
    }
}