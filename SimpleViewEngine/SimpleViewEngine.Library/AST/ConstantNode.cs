namespace Renfield.SimpleViewEngine.Library.AST
{
  public class ConstantNode : Node
  {
    public ConstantNode(string text)
    {
      this.text = text;
    }

    public override string Eval(object model)
    {
      return text;
    }

    //

    private readonly string text;
  }
}