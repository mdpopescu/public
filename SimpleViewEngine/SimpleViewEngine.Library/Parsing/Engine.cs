using System;
using System.Collections.Generic;
using System.Linq;
using Renfield.SimpleViewEngine.Library.AST;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class Engine
  {
    public Engine(Func<string, IEnumerable<Node>> templateParser)
    {
      this.templateParser = templateParser;
      templateCache = new Dictionary<string, IEnumerable<Node>>();
    }

    public string Run(IEnumerable<Node> nodes, object model)
    {
      IncludeNode.EvalTemplate = (templateName, m) =>
      {
        var innerNodes = GetNodes(templateName);
        return Run(innerNodes, m);
      };

      return string.Join("", Process(nodes, model));
    }

    //

    private readonly Func<string, IEnumerable<Node>> templateParser;
    private readonly Dictionary<string, IEnumerable<Node>> templateCache;

    private static IEnumerable<string> Process(IEnumerable<Node> nodes, object model)
    {
      return nodes.Select(node => node.Eval(model));
    }

    private IEnumerable<Node> GetNodes(string templateName)
    {
      if (!templateCache.ContainsKey(templateName))
        templateCache[templateName] = templateParser(templateName);

      return templateCache[templateName];
    }
  }
}