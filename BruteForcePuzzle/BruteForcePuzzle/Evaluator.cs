using System;

namespace Renfield.BruteForcePuzzle
{
  public class Evaluator
  {
    public Evaluator(Parser parser)
    {
      this.parser = parser;
    }

    public int Eval(string s)
    {
      if (string.IsNullOrEmpty(s))
        throw new Exception();
      var elements = new ElementList(parser.Parse(s));
      var operation = elements.FindOperation();
      while (operation != null)
      {
        var newElement = operation.Compute();
        elements.ReplaceOperation(operation, newElement);
        operation = elements.FindOperation();
      }

      return elements.First.Value;
    }

    //

    private readonly Parser parser;
  }
}