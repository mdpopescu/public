using System;
using System.Collections.Generic;
using System.Linq;

namespace Renfield.BruteForcePuzzle
{
  public class Parser
  {
    public Parser(OperatorFactory operatorFactory, IOperandFactory operandFactory,
                  IDictionary<string, int> symbols = null)
    {
      this.operatorFactory = operatorFactory;
      this.operandFactory = operandFactory;
      this.symbols = symbols;
    }

    public IEnumerable<Element> Parse(string s)
    {
      const int BOOST = 10;
      var precedenceBoost = 0;
      var operand = "";
      foreach (var currentChar in s.Where(c => !char.IsWhiteSpace(c)))
      {
        if (char.IsLetterOrDigit(currentChar) || currentChar == '.')
          operand += currentChar;
        else
        {
          if (operand != "")
            yield return operandFactory.Create(GetOperand(operand));
          operand = "";
          switch (currentChar)
          {
            case '(':
              precedenceBoost += BOOST;
              break;
            case ')':
              precedenceBoost -= BOOST;
              break;
            default:
              yield return operatorFactory.Create(currentChar, precedenceBoost);
              break;
          }
        }
      }
      if (operand != "")
        yield return operandFactory.Create(GetOperand(operand));
      if (precedenceBoost > 0)
        throw new Exception("Too many open parentheses");
      if (precedenceBoost < 0)
        throw new Exception("Too many closed parentheses");
    }

    //
    private readonly OperatorFactory operatorFactory;
    private readonly IOperandFactory operandFactory;
    private readonly IDictionary<string, int> symbols;

    private int GetOperand(string operand)
    {
      return char.IsLetter(operand.First())
               ? symbols[operand]
               : Convert.ToInt32(operand);
    }
  }
}