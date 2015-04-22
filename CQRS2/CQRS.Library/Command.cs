using System.Reflection;

namespace CQRS.Library
{
  public static class Command
  {
    public static void Send(object target, string name, params object[] args)
    {
      var type = target.GetType();
      var method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
      method.Invoke(target, args);
    }
  }
}