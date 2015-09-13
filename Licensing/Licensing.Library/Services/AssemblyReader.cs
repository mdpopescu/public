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

    public string GetPath()
    {
      var company = GetAttribute<AssemblyCompanyAttribute>(it => it.Company);
      var product = GetAttribute<AssemblyProductAttribute>(it => it.Product);

      return string.Format(@"Software\{0}\{1}", company, product);
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