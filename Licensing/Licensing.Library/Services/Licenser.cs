using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class Licenser
  {
    public Licenser(LicenserOptions options, Storage storage)
    {
      this.options = options;
      this.storage = storage;
    }

    public bool Check()
    {
      storage.Load();
      return false;
    }

    //

    private readonly LicenserOptions options;
    private readonly Storage storage;
  }
}