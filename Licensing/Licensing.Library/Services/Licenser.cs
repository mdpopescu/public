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
      var subkey = reader.GetPath();
      var key = Registry.LocalMachine.OpenSubKey(subkey) ?? Registry.LocalMachine.CreateSubKey(subkey);
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