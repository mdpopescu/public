namespace Renfield.Licensing.Library.Contracts
{
  public interface LicenseChecker : RemoteChecker
  {
    bool IsLicensed { get; }
    bool IsTrial { get; }
  }
}