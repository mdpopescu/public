using System;
using System.Threading.Tasks;

namespace CQRS.Library
{
  public static class Command
  {
    public static Task SendAsync<T>(T target, Action<T> action)
    {
      return MethodCaller.CallActionAsync(() => action(target));
    }
  }
}