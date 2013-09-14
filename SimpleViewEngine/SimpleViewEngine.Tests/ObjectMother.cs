using System.Linq;
using Renfield.SimpleViewEngine.Library.Parsing;

namespace Renfield.SimpleViewEngine.Tests
{
  public static class ObjectMother
  {
    public static TokenList CreateEmptyTokenList()
    {
      return TokenList.Create(Enumerable.Empty<Token>());
    }

    public static TokenList CreateTokenListWithConstantNode()
    {
      return TokenList.Create(new[] {new Token("constant", "test", null)});
    }

    public static TokenList CreateTokenListWithOnePropertyNode()
    {
      return TokenList.Create(new[] {new Token("property", "{{a}}", null)});
    }

    public static TokenList CreateTokenListWithOnePropertyNodeEvaluatingSelf()
    {
      return TokenList.Create(new[] {new Token("property", "{{}}", null)});
    }

    public static TokenList CreateTokenListWithEofInMiddle()
    {
      return TokenList.Create(
        new[]
        {
          new Token("property", "{{a}}", null),
          new Token("(eof)", null, null),
          new Token("constant", "test", null),
        });
    }

    public static TokenList CreateTokenListWithUnknownToken()
    {
      return TokenList.Create(new[] {new Token("unknown", "abc", null),});
    }

    public static TokenList CreateTokenListWithIf()
    {
      return TokenList.Create(
        new[]
        {
          new Token("if", "{{if b}}", null),
          new Token("constant", "test", null),
          new Token("endif", "{{endif}}", null),
        });
    }

    public static TokenList CreateTokenListWithIfElse()
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

    public static TokenList CreateTokenListWithForEach()
    {
      return TokenList.Create(
        new[]
        {
          new Token("foreach", "{{foreach a}}", null),
          new Token("constant", "test", null),
          new Token("endfor", "{{endfor}}", null),
        });
    }
  }
}