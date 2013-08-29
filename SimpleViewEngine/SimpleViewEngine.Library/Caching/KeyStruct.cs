using System;
using System.Linq;

namespace Renfield.SimpleViewEngine.Library.Caching
{
  public class KeyStruct
  {
    public KeyStruct(params object[] args)
    {
      if (args == null)
        throw new ArgumentNullException("args");

      this.args = args.Select(arg => arg ?? 0).ToArray();
    }

    public override bool Equals(object obj)
    {
      if (!(obj is KeyStruct))
        return false;

      var compareTo = (KeyStruct) obj;
      if (args.Length != compareTo.args.Length)
        return false;

      return args
        .Zip(compareTo.args, (o1, o2) => new {o1, o2})
        .All(pair => pair.o1.Equals(pair.o2));
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return args.Aggregate(17, (acc, arg) => acc * 23 + arg.GetHashCode());
      }
    }

    public static bool operator ==(KeyStruct obj1, KeyStruct obj2)
    {
      return obj1 == null ? obj2 == null : obj1.Equals(obj2);
    }

    public static bool operator !=(KeyStruct obj1, KeyStruct obj2)
    {
      return !(obj1 == obj2);
    }

    //

    private readonly object[] args;
  }
}