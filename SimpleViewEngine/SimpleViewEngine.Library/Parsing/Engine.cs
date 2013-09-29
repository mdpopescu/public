using System;
using System.Collections.Generic;
using System.Linq;
using Renfield.SimpleViewEngine.Library.AST;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class Engine
  {
    public Func<string, IEnumerable<Node>> ParseTemplate { get; set; }

    public Engine()
    {
      templateCache = new Dictionary<string, IEnumerable<Node>>();
    }

    public string Run(IEnumerable<Node> nodes, object model)
    {
      nodes = nodes.ToList();

      foreach (var node in nodes.OfType<IncludeNode>())
      {
        node.EvalTemplate = (templateName, m) =>
        {
          var innerNodes = GetNodes(templateName);
          return Run(innerNodes, m);
        };
      }

      return string.Join("", Process(nodes, model));
    }

    //

    private readonly Dictionary<string, IEnumerable<Node>> templateCache;

    private static IEnumerable<string> Process(IEnumerable<Node> nodes, object model)
    {
      return nodes.Select(it => it.Eval(model));
    }

    private IEnumerable<Node> GetNodes(string templateName)
    {
      if (!templateCache.ContainsKey(templateName))
        templateCache[templateName] = ParseTemplate(templateName);

      return templateCache[templateName];
    }
  }
}