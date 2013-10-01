namespace Renfield.BruteForcePuzzle
{
  public class DivOperator : Operator
  {
    public DivOperator(int precedenceBoost = 0)
    {
      Precedence = 2 + precedenceBoost;
    }

    public override int Compute(Operand left, Operand right)
    {
      return left.Value / right.Value;
    }
  }
}