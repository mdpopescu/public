using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class OptionsDetailsReader : DetailsReader
  {
    public OptionsDetailsReader(LicenseOptions options)
    {
      this.options = options;
    }

    public Details Read()
    {
      return new Details
      {
        Company = options.Company,
        Product = options.Product,
      };
    }

    //

    private readonly LicenseOptions options;
  }
}