using DecoratorGen.Library.Models;

namespace DecoratorGen.Library.Contracts
{
    public interface IMemberGenerator
    {
        string Generate(Member member);
    }
}