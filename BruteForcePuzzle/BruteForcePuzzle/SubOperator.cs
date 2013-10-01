namespace Renfield.BruteForcePuzzle
{
  public class SubOperator : Operator
  {
    public SubOperator(int precedenceBoost = 0)
    {
      Precedence = 1 + precedenceBoost;
    }

    public override int Compute(Operand left, Operand right)
    {
      return left.Value - right.Value;
    }
  }
}