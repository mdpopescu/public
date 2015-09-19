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
      registration = LoadOrCreate();
      CheckStatus();

      return registration;
    }

    public void SaveRegistration(LicenseRegistration details)
    {
      // overwrite the currently saved registration information
      registration = details;
      storage.Save(registration);

      checker.Submit(registration);

      CheckStatus();
    }

    //

    protected Licenser(Storage storage, LicenseChecker checker)
    {
      this.storage = storage;
      this.checker = checker;
    }

    protected void Initialize()
    {
      registration = LoadOrCreate();
      CheckStatus();

      UpdateRemainingRuns();
    }

    //

    private readonly Storage storage;
    private readonly LicenseChecker checker;

    private LicenseRegistration registration;

    private LicenseRegistration LoadOrCreate()
    {
      registration = storage.Load();
      if (registration == null)
      {
        registration = new LicenseRegistration();
        storage.Save(registration);
      }

      return registration;
    }

    private void CheckStatus()
    {
      var oldExpiration = registration.Expiration;

      checker.Check(registration);

      if (registration.Expiration != oldExpiration)
        storage.Save(registration);
    }

    private void UpdateRemainingRuns()
    {
      if (registration.Limits.Runs <= 0)
        return;

      registration.Limits.Runs--;
      storage.Save(registration);
    }
  }
}