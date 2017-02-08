using ExpressionCompiler.Models;

namespace ExpressionCompiler.Contracts
{
    public interface IBoostable
    {
        Token Boost(int boost);
    }
}