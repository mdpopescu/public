using Renfield.SimpleViewEngine.Library.Reflection;

namespace Renfield.SimpleViewEngine.Library.AST
{
  public abstract class Node
  {
    public const string SELF = "it";

    public abstract string Eval(object model);

    //

    protected virtual object GetProperty(object model, string name)
    {
      if (name == SELF)
        return model.ToString();

      var func = model.GetPropertyFunc();
      return func(name);
    }
  }
}