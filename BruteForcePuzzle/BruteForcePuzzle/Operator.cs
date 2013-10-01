namespace Renfield.BruteForcePuzzle
{
  public abstract class Operator : Element
  {
    public int Precedence { get; protected set; }

    public abstract int Compute(Operand left, Operand right);
  }
}