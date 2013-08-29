using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Renfield.SimpleViewEngine.Library.AST
{
  public class Lexer : ILexer
  {
    public Lexer()
    {
      tokenDefinitions = new List<TokenDefinition>();
      endOfLineRegex = new Regex(Environment.NewLine);
    }

    public void AddDefinition(TokenDefinition tokenDefinition)
    {
      tokenDefinitions.Add(tokenDefinition);
    }

    public IEnumerable<Token> Tokenize(string source)
    {
      var currentIndex = 0;
      var currentLine = 1;
      var currentColumn = 0;

      while (currentIndex < source.Length)
      {
        TokenDefinition matchedDefinition = null;
        var matchLength = 0;

        foreach (var rule in tokenDefinitions)
        {
          var match = rule.Regex.Match(source, currentIndex);
          if (match.Success && (match.Index - currentIndex) == 0)
          {
            matchedDefinition = rule;
            matchLength = match.Length;
            break;
          }
        }

        if (matchedDefinition == null)
          throw new Exception(string.Format("Unrecognized symbol '{0}' at index {1} (line {2}, column {3}).",
            source[currentIndex], currentIndex, currentLine, currentColumn));

        var value = source.Substring(currentIndex, matchLength);

        if (!matchedDefinition.IsIgnored)
          yield return
            new Token(matchedDefinition.Type, value, new TokenPosition(currentIndex, currentLine, currentColumn));

        var endOfLineMatch = endOfLineRegex.Match(value);
        if (endOfLineMatch.Success)
        {
          currentLine += 1;
          currentColumn = value.Length - (endOfLineMatch.Index + endOfLineMatch.Length);
        }
        else
        {
          currentColumn += matchLength;
        }

        currentIndex += matchLength;
      }

      yield return new Token("(end)", null, new TokenPosition(currentIndex, currentLine, currentColumn));
    }

    //

    private readonly List<TokenDefinition> tokenDefinitions;
    private readonly Regex endOfLineRegex;
  }
}