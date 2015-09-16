using System;
using Microsoft.Win32;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Extensions;
using Renfield.Licensing.Library.Models;
using Renfield.Licensing.Library.Services.Validators;

namespace Renfield.Licensing.Library.Services
{
  public class Licenser
  {
    public static Licenser Create(LicenseOptions options)
    {
      var reader = new AssemblyReader();
      reader.ReadValues();

      var company = reader.Company.NullIfEmpty() ?? options.Company ?? "[Company Name]";
      var product = reader.Product.NullIfEmpty() ?? options.Product ?? "[Company Name]";
      PathBuilder pathBuilder = new PathBuilderImpl();
      var subkey = pathBuilder.GetPath(company, product);
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

    public bool IsLicensed()
    {
      var registration = storage.Load();
      if (registration == null)
        return false;

      if (!validator.Isvalid(registration))
        return false;

      if (Remote != null)
      {
        var response = GetRemoteResponse(registration.Key);
        return CheckRemoteResponse(response, registration);
      }

      return true;
    }

    public bool IsTrial()
    {
      if (IsLicensed())
        return true;

      var registration = storage.Load();
      if (registration == null)
        return false;

      if (registration.Limits == null)
        return false;

      if (registration.Limits.Days != -1 && registration.CreatedOn.AddDays(registration.Limits.Days) < DateTime.Today)
        return false;

      if (registration.Limits.Runs == 0)
        return false;

      if (registration.Limits.Runs > 0)
        UpdateRemainingRuns(registration);

      return true;
    }

    public bool ShouldRun()
    {
      return IsLicensed() || IsTrial();
    }

    public LicenseRegistration GetRegistration()
    {
      return storage.Load();
    }

    public void SaveRegistration(LicenseRegistration registration)
    {
      registration.ProcessorId = sys.GetProcessorId();

      var fields = registration.GetLicenseFields();
      var data = WebTools.FormUrlEncoded(fields);

      var ok = true;

      if (Remote != null)
      {
        var response = Remote.Post(data);
        ok = CheckRemoteResponse(response, registration);
      }

      if (ok)
        storage.Save(registration);
    }

    //

    private readonly Storage storage;
    private readonly Sys sys;
    private readonly Validator validator;

    private string GetRemoteResponse(string key)
    {
      var processorId = sys.GetProcessorId();
      var query = string.Format("Key={0}&ProcessorId={1}", key, processorId);

      try
      {
        return Remote.Get(query);
      }
      catch
      {
        return null;
      }
    }

    private bool CheckRemoteResponse(string response, LicenseRegistration registration)
    {
      var parsed = ResponseParser.Parse(response);
      if (parsed.Key != registration.Key)
        return false;

      UpdateExpirationDate(registration, parsed.Expiration);

      return DateTime.Today <= parsed.Expiration;
    }

    private void UpdateExpirationDate(LicenseRegistration registration, DateTime expiration)
    {
      registration.Expiration = expiration;
      storage.Save(registration);
    }

    private void UpdateRemainingRuns(LicenseRegistration registration)
    {
      registration.Limits.Runs--;
      storage.Save(registration);
    }
  }
}