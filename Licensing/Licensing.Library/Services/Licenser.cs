using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class Licenser
  {
    /// <summary>
    ///   Factory method
    /// </summary>
    /// <param name="options">The license options</param>
    /// <returns>A new Licenser</returns>
    public static Licenser Create(LicenseOptions options)
    {
      var storage = Bootstrapper.GetStorage(options);
      var checker = Bootstrapper.GetLicenseChecker(options);

      var licenser = new Licenser(storage, checker);
      licenser.Initialize();

      return licenser;
    }

    //

    public bool IsLicensed
    {
      get { return checker.IsLicensed; }
    }

    public bool IsTrial
    {
      get { return checker.IsTrial; }
    }

    public bool ShouldRun
    {
      get { return IsLicensed || IsTrial; }
    }

    public LicenseRegistration LoadRegistration()
    {
      var details = LoadOrCreate();
      CheckStatus(details);

      return details;
    }

    public void SaveRegistration(LicenseRegistration details)
    {
      // overwrite the currently saved registration information
      storage.Save(details);
      checker.Submit(details);

      CheckStatus(details);
    }

    //

    protected Licenser(Storage storage, LicenseChecker checker)
    {
      this.storage = storage;
      this.checker = checker;
    }

    protected void Initialize()
    {
      var details = LoadOrCreate();
      CheckStatus(details);

      UpdateRemainingRuns(details);
    }

    //

    private readonly Storage storage;
    private readonly LicenseChecker checker;

    private LicenseRegistration LoadOrCreate()
    {
      var details = storage.Load();
      if (details == null)
      {
        details = new LicenseRegistration();
        storage.Save(details);
      }

      return details;
    }

    private void CheckStatus(LicenseRegistration details)
    {
      var oldExpiration = details.Expiration;

      checker.Check(details);

      if (details.Expiration != oldExpiration)
        storage.Save(details);
    }

    private void UpdateRemainingRuns(LicenseRegistration details)
    {
      if (details.Limits.Runs <= 0)
        return;

      details.Limits.Runs--;
      storage.Save(details);
    }
  }
}