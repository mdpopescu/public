using System;
using System.Reflection;

namespace Renfield.Licensing.Library.Services
{
  public class AssemblyReader
  {
    public AssemblyReader()
    {
      assembly = Assembly.GetEntryAssembly();
    }

    public string Company { get; private set; }
    public string Product { get; private set; }

    public void ReadValues()
    {
      Company = GetAttribute<AssemblyCompanyAttribute>(it => it.Company);
      Product = GetAttribute<AssemblyProductAttribute>(it => it.Product);
    }

    //

    private readonly Assembly assembly;

    private string GetAttribute<T>(Func<T, string> func)
    {
      var attributes = assembly.GetCustomAttributes(typeof (T), false);
      var value = attributes.Length > 0 ? func((T) attributes[0]) : "";

      return (value + "").Trim();
    }
  }
}