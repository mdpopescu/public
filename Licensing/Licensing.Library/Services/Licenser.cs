using System;
using Microsoft.Win32;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class Licenser
  {
    public static Licenser Create(LicenseOptions options)
    {
      var reader = new AssemblyReader();
      var key = Registry.LocalMachine.OpenSubKey(reader.GetPath());
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

      return new Licenser(storage, sys) {Remote = remote, ResponseParser = parser};
    }

    public Licenser(Storage storage, Sys sys)
    {
      this.storage = storage;
      this.sys = sys;
    }

    public Remote Remote { get; set; }
    public ResponseParser ResponseParser { get; set; }

    public bool IsLicensed()
    {
      var registration = storage.Load();
      if (registration == null)
        return false;

      if (!registration.IsValidLicense())
        return false;

      if (Remote != null)
        return CheckRemoteResponse(registration);

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

    public void CreateRegistration(LicenseRegistration registration)
    {
      var fields = registration.GetLicenseFields();
      var data = WebTools.FormUrlEncoded(fields);

      Remote.Post(data);
    }

    //

    private readonly Storage storage;
    private readonly Sys sys;

    private bool CheckRemoteResponse(LicenseRegistration registration)
    {
      try
      {
        var response = GetRemoteResponse(registration.Key);
        var parsed = ResponseParser.Parse(response);
        if (parsed.Key != registration.Key)
          return false;

        UpdateExpirationDate(registration, parsed.Expiration);

        return DateTime.Today <= parsed.Expiration;
      }
      catch
      {
        return false;
      }
    }

    private string GetRemoteResponse(string key)
    {
      var processorId = sys.GetProcessorId();
      var address = string.Format("Key={0}&ProcessorId={1}", key, processorId);

      return Remote.Get(address);
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