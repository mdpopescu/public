using System.Reflection;

namespace CQRS.Library
{
  public static class Extensions
  {
    public static void SendCommand(this object target, string name, params object[] args)
    {
      Command.Send(target, name, args);
    }

    public static MethodInfo FindMethod(this object target, string name)
    {
      var type = target.GetType();
      return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
    }
  }
}