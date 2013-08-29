namespace Renfield.SimpleViewEngine.Library
{
  public class ConstantNode : Node
  {
    public ConstantNode(string text)
    {
      this.text = text;
    }

    public override string Eval(dynamic model)
    {
      return text;
    }

    //

    private readonly string text;
  }
}