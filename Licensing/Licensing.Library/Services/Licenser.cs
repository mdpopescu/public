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

      Encryptor encryptor = string.IsNullOrWhiteSpace(options.Password) || string.IsNullOrWhiteSpace(options.Salt)
        ? null
        : new RijndaelEncryptor(options.Password, options.Salt);
      Serializer<LicenseRegistration> serializer = new LicenseSerializer();
      Storage storage = new SecureStorage(io, serializer) {Encryptor = encryptor};

      Sys sys = new WinSys();

      Remote remote = string.IsNullOrWhiteSpace(options.CheckUrl)
        ? null
        : new WebRemote("https://" + options.CheckUrl);
      ResponseParser parser = remote == null
        ? null
        : new ResponseParserImpl();

      Validator chain = new GuidValidator(it => it.Key,
        new NonEmptyValidator(it => it.Name,
          new NonEmptyValidator(it => it.Contact,
            new NonEmptyValidator(it => it.ProcessorId,
              new ExpirationValidator(it => it.Expiration,
                null)))));

      return new Licenser(storage, sys, chain) {Remote = remote, ResponseParser = parser};
    }

    public Licenser(Storage storage, Sys sys, Validator validator)
    {
      this.storage = storage;
      this.sys = sys;
      this.validator = validator;
    }

    public Remote Remote { get; set; }
    public ResponseParser ResponseParser { get; set; }

    public bool IsLicensed { get; private set; }
    public bool IsTrial { get; private set; }

    public bool ShouldRun
    {
      get { return IsLicensed || IsTrial; }
    }

    public void Initialize()
    {
      registration = storage.Load();
      if (registration == null)
      {
        registration = new LicenseRegistration {ProcessorId = sys.GetProcessorId()};
        storage.Save(registration);
      }

      SetIsLicensed();
      SetIsTrial();

      UpdateRemainingRuns();
    }

    public LicenseRegistration GetRegistration()
    {
      return registration;
    }

    public void SaveRegistration(LicenseRegistration details)
    {
      // overwrite the currently saved registration information
      registration = details;

      details.ProcessorId = sys.GetProcessorId();

      var fields = details.GetLicenseFields();

      var ok = true;

      if (Remote != null)
      {
        var data = sys.Encode(fields);
        var response = Remote.Post(data);
        ok = CheckRemoteResponse(response);
      }

      if (ok)
        storage.Save(details);
    }

    //

    private readonly Storage storage;
    private readonly Sys sys;
    private readonly Validator validator;

    private LicenseRegistration registration;

    private void SetIsLicensed()
    {
      if (registration == null)
        IsLicensed = false;
      else if (!validator.Isvalid(registration))
        IsLicensed = false;
      else if (Remote == null)
        IsLicensed = true;
      else
      {
        var response = GetRemoteResponse();
        IsLicensed = CheckRemoteResponse(response);
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

    private string GetRemoteResponse()
    {
      var processorId = sys.GetProcessorId();
      var query = string.Format("Key={0}&ProcessorId={1}", registration.Key, processorId);

      return Remote.Get(query);
    }

    private bool CheckRemoteResponse(string response)
    {
      var parsed = ResponseParser.Parse(response);
      if (parsed.Key != registration.Key)
        return false;

      UpdateExpirationDate(parsed.Expiration);

      return DateTime.Today <= parsed.Expiration;
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