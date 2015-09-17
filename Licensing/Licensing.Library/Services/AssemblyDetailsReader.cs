using System;
using System.Reflection;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class AssemblyDetailsReader : DetailsReader
  {
    public AssemblyDetailsReader(Assembly assembly)
    {
      this.assembly = assembly;
    }

    public Details Read()
    {
      return new Details
      {
        Company = GetAttribute<AssemblyCompanyAttribute>(it => it.Company),
        Product = GetAttribute<AssemblyProductAttribute>(it => it.Product),
      };
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