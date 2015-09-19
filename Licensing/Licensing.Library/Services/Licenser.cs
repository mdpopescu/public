using System;
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
      var details = Bootstrapper.LoadRegistration(options);

      var storage = Bootstrapper.GetStorage(options, details);
      var sys = new WinSys();
      var checker = Bootstrapper.GetChecker(options, sys);
      var chain = Bootstrapper.GetValidator();

      var licenser = new Licenser(storage, sys, checker, chain);
      licenser.Initialize();

      return licenser;
    }

    //

    public bool IsLicensed { get; private set; }
    public bool IsTrial { get; private set; }

    public bool ShouldRun
    {
      get { return IsLicensed || IsTrial; }
    }

    public LicenseRegistration GetRegistration()
    {
      return registration;
    }

    public void SaveRegistration(LicenseRegistration details)
    {
      // overwrite the currently saved registration information
      registration = details;
      registration.ProcessorId = sys.GetProcessorId();

      if (!validator.Isvalid(registration))
        return;

      var expiration = checker.Submit(registration);
      if (!CheckRemoteResponse(expiration))
        return;

      // this checks remote again, with a GET this time
      CheckLicenseStatus();
      storage.Save(registration);
    }

    //

    protected Licenser(Storage storage, Sys sys, RemoteChecker checker, Validator validator)
    {
      this.storage = storage;
      this.sys = sys;
      this.checker = checker;
      this.validator = validator;
    }

    protected void Initialize()
    {
      registration = storage.Load();
      if (registration == null)
      {
        registration = new LicenseRegistration {ProcessorId = sys.GetProcessorId()};
        storage.Save(registration);
      }

      CheckLicenseStatus();
      UpdateRemainingRuns();
    }

    //

    private readonly Storage storage;
    private readonly Sys sys;
    private readonly RemoteChecker checker;
    private readonly Validator validator;

    private LicenseRegistration registration;

    private void CheckLicenseStatus()
    {
      SetIsLicensed();
      SetIsTrial();
    }

    private void SetIsLicensed()
    {
      if (registration == null)
        IsLicensed = false;
      else if (!validator.Isvalid(registration))
        IsLicensed = false;
      else
      {
        var expiration = checker.Check(registration);
        IsLicensed = CheckRemoteResponse(expiration);
      }
    }

    private void SetIsTrial()
    {
      if (IsLicensed)
        IsTrial = true;
      else if (registration == null)
        IsTrial = false;
      else if (registration.Limits == null)
        IsTrial = false;
      else if (registration.Limits.Days != -1 && registration.CreatedOn.AddDays(registration.Limits.Days) < DateTime.Today)
        IsTrial = false;
      else if (registration.Limits.Runs == 0)
        IsTrial = false;
      else
        IsTrial = true;
    }

    private bool CheckRemoteResponse(DateTime? expiration)
    {
      if (expiration == null)
        return false;

      UpdateExpirationDate(expiration.Value);
      return DateTime.Today <= expiration.Value;
    }

    private void UpdateExpirationDate(DateTime expiration)
    {
      registration.Expiration = expiration;
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