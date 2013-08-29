using System.Collections.Generic;

namespace Renfield.SimpleViewEngine.Library.AST
{
  public abstract class Node
  {
    public abstract string Eval(object model);

    //

    protected virtual string GetProperty(object model, string name)
    {
      var dict = model as IDictionary<string, object>;
      if (dict != null)
        return dict[name].ToString();

      var ps = model.GetType().GetProperty(name);
      return ps.GetValue(model).ToString();
    }
  }
}