using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Contracts
{
  public interface RequestBuilder
  {
    string BuildQuery(LicenseRegistration registration);
    string BuildData(LicenseRegistration registration);
  }
}