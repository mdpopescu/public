namespace CQRS.Library
{
  public static class Extensions
  {
    public static void SendCommand(this object target, string name, params object[] args)
    {
      Command.Send(target, name, args);
    }
  }
}