using System;
using System.Collections.Generic;
using Renfield.SimpleViewEngine.Library.AST;

namespace Renfield.SimpleViewEngine.Library
{
  public class Parser
  {
    public IEnumerable<Node> Parse(IEnumerable<Token> tokens)
    {
      foreach (var token in tokens)
      {
        switch (token.Type)
        {
          case "constant":
            yield return new ConstantNode(token.Value);
            break;

          case "property":
            yield return new PropertyNode(token.Value.Substring(2, token.Value.Length - 4));
            break;

          case "(eof)":
            yield break;

          default:
            throw new Exception(string.Format("Unexpected token of type [{0}] and value [{1}]", token.Type, token.Value));
        }
      }
    }
  }
}