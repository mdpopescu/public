using System;
using System.Reflection;
using Microsoft.Win32;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;
using Renfield.Licensing.Library.Services.Validators;

namespace Renfield.Licensing.Library.Services
{
  public class Licenser
  {
    public static Licenser Create(LicenseOptions options)
    {
      DetailsReader r1 = new OptionsDetailsReader(options);
      DetailsReader r2 = new AssemblyDetailsReader(Assembly.GetEntryAssembly());
      DetailsReader reader = new CompositeDetailsReader(r1, r2);
      var details = reader.Read();

      PathBuilder pathBuilder = new RegistryPathBuilder();
      var subkey = pathBuilder.GetPath(details.Company, details.Product);
      var key = Registry.CurrentUser.OpenSubKey(subkey, RegistryKeyPermissionCheck.ReadWriteSubTree)
                ?? Registry.CurrentUser.CreateSubKey(subkey, RegistryKeyPermissionCheck.ReadWriteSubTree);
      StringIO io = new RegistryIO(key);

      var encryptor = string.IsNullOrWhiteSpace(options.Password) || string.IsNullOrWhiteSpace(options.Salt)
        ? (Encryptor) new NullEncryptor()
        : new RijndaelEncryptor(options.Password, options.Salt);
      Serializer<LicenseRegistration> serializer = new LicenseSerializer();
      Storage storage = new SecureStorage(io, encryptor, serializer);

      Sys sys = new WinSys();

      RemoteChecker checker;
      if (string.IsNullOrWhiteSpace(options.CheckUrl))
      {
        checker = new NullRemoteChecker();
      }
      else
      {
        Remote remote = new WebRemote("https://" + options.CheckUrl);
        ResponseParser parser = new ResponseParserImpl();
        checker = string.IsNullOrWhiteSpace(options.CheckUrl)
          ? (RemoteChecker) new NullRemoteChecker()
          : new RemoteCheckerClient(sys, remote, parser);
      }

      Validator chain = new GuidValidator(it => it.Key,
        new NonEmptyValidator(it => it.Name,
          new NonEmptyValidator(it => it.Contact,
            new NonEmptyValidator(it => it.ProcessorId,
              new ExpirationValidator(it => it.Expiration,
                null)))));

      var licenser = new Licenser(storage, sys, checker, chain);
      licenser.Initialize();

      return licenser;
    }

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