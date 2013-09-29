using System;

namespace Renfield.SimpleViewEngine.Library.AST
{
  public class IncludeNode : PropertyNode
  {
    public static Func<string, dynamic, string> EvalTemplate { get; set; }

    public IncludeNode(string name, string templateName)
      : base(name)
    {
      this.templateName = templateName;
    }

    public override string Eval(object model)
    {
      return EvalTemplate(templateName, GetProperty(model, name));
    }

    //

    private readonly string templateName;
  }
}