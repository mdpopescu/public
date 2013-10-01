using System;

namespace Renfield.BruteForcePuzzle
{
  public class OperatorFactory
  {
    public Operator Create(char op, int precedenceBoost)
    {
      switch (op)
      {
        case '+':
          return new AddOperator(precedenceBoost);
        case '-':
          return new SubOperator(precedenceBoost);
        case '*':
          return new MulOperator(precedenceBoost);
        case '/':
          return new DivOperator(precedenceBoost);
        default:
          throw new Exception(string.Format("Unknown operator [{0}]", op));
      }
    }
  }
}