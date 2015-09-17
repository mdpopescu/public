using System;
using System.Collections.Generic;
using System.Linq;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class CompositeDetailsReader : DetailsReader
  {
    public CompositeDetailsReader(params DetailsReader[] readers)
    {
      this.readers = readers;
    }

    public Details Read()
    {
      var list = readers.Select(it => it.Read()).ToList();

      return new Details
      {
        Company = FirstNonEmpty(list, it => it.Company),
        Product = FirstNonEmpty(list, it => it.Product),
      };
    }

    //

    private readonly DetailsReader[] readers;

    private static string FirstNonEmpty(IEnumerable<Details> list, Func<Details, string> selector)
    {
      return list.Select(selector).Where(it => !string.IsNullOrWhiteSpace(it)).FirstOrDefault() ?? "";
    }
  }
}