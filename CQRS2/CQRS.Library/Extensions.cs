using System;
using System.Reflection;
using System.Threading.Tasks;

namespace CQRS.Library
{
  public static class Extensions
  {
    public static Task SendCommandAsync<T>(this T target, Action<T> action)
    {
      return Command.SendAsync(target, action);
    }

    public static MethodInfo FindMethod(this object target, string name)
    {
      var type = target.GetType();
      return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
    }
  }
}