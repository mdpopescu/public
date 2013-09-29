using System.Linq;
using Renfield.SimpleViewEngine.Library.Parsing;

namespace Renfield.SimpleViewEngine.Tests
{
  public static class ObjectMother
  {
    public static TokenList CreateEmptyList()
    {
      return TokenList.Create(Enumerable.Empty<Token>());
    }

    public static TokenList CreateConstantNode()
    {
      return TokenList.Create(new[] {new Token("constant", "test", null)});
    }

    public static TokenList CreateOnePropertyNode()
    {
      return TokenList.Create(new[] {new Token("property", "{{a}}", null)});
    }

    public static TokenList CreateOnePropertyNodeEvaluatingSelf()
    {
      return TokenList.Create(new[] {new Token("property", "{{}}", null)});
    }

    public static TokenList CreateEofInMiddle()
    {
      return TokenList.Create(
        new[]
        {
          new Token("property", "{{a}}", null),
          new Token("(eof)", null, null),
          new Token("constant", "test", null),
        });
    }

    public static TokenList CreateUnknownToken()
    {
      return TokenList.Create(new[] {new Token("unknown", "abc", null),});
    }

    public static TokenList CreateIf()
    {
      return TokenList.Create(
        new[]
        {
          new Token("if", "{{if b}}", null),
          new Token("constant", "test", null),
          new Token("endif", "{{endif}}", null),
        });
    }

    public static TokenList CreateIfElse()
    {
      return TokenList.Create(
        new[]
        {
          new Token("if", "{{if b}}", null),
          new Token("constant", "test1", null),
          new Token("else", "{{else}}", null),
          new Token("constant", "test2", null),
          new Token("endif", "{{endif}}", null),
        });
    }

    public static TokenList CreateForEach()
    {
      return TokenList.Create(
        new[]
        {
          new Token("foreach", "{{foreach a}}", null),
          new Token("constant", "test", null),
          new Token("endfor", "{{endfor}}", null),
        });
    }

    public static TokenList CreateInclude()
    {
      return TokenList.Create(
        new[]
        {
          new Token("include", "{{include other a}}", null),
        });
    }
  }
}