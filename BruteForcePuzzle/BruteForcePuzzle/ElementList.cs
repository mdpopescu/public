using System.Collections.Generic;
using System.Linq;

namespace Renfield.BruteForcePuzzle
{
  public class ElementList
  {
    public Operand First
    {
      get { return elements[0] as Operand; }
    }

    public ElementList(IEnumerable<Element> elements)
    {
      this.elements = new List<Element>(elements);
    }

    public Operation FindOperation()
    {
      var operators = elements.Where(el => el is Operator).Cast<Operator>().ToList();
      if (!operators.Any())
        return null;
      
      // I don't know if OrderByDescending is stable so I won't use that
      var maxPrecedence = operators.Max(op => op.Precedence);
      var firstOp = operators.First(op => op.Precedence == maxPrecedence);
      var index = elements.IndexOf(firstOp);

      return new Operation(GetOperand(index - 1), elements[index] as Operator, GetOperand(index + 1));
    }

    public void ReplaceOperation(Operation operation, Operand operand)
    {
      var index = elements.IndexOf(operation.Op);
      if (GetOperand(index + 1) == operation.ROperand)
        elements.RemoveAt(index + 1);
      
      elements[index] = operand;
      if (GetOperand(index - 1) == operation.LOperand)
        elements.RemoveAt(index - 1);
    }

    //

    private readonly IList<Element> elements;

    private Operand GetOperand(int index)
    {
      if (index >= 0 && index < elements.Count && elements[index] is Operand)
        return (Operand) elements[index];

      return new Operand(0);
    }
  }
}