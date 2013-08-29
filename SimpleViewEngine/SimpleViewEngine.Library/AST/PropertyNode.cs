namespace Renfield.SimpleViewEngine.Library.AST
{
  public class PropertyNode : Node
  {
    public PropertyNode(string name)
    {
      this.name = name;
    }

    public override string Eval(object model)
    {
      return GetProperty(model, name);
    }

    //

    private readonly string name;
  }
}