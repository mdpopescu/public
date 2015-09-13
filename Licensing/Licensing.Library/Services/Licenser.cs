﻿using System;
using System.Globalization;
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

      Encryptor encryptor = new RijndaelEncryptor(options.Password);
      Serializer<LicenseRegistration> serializer = new LicenseSerializer();
      Storage storage = new SecureStorage(io, encryptor, serializer);

      Sys sys = new WinSys();

      Remote remote = string.IsNullOrWhiteSpace(options.CheckUrl)
        ? null
        : new WebRemote("https://" + options.CheckUrl);

      return new Licenser(storage, sys) {Remote = remote};
    }

    public Licenser(Storage storage, Sys sys)
    {
      this.storage = storage;
      this.sys = sys;
    }

    public Remote Remote { get; set; }

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

    //

    private readonly Storage storage;
    private readonly Sys sys;

    private bool CheckRemoteResponse(LicenseRegistration registration)
    {
      try
      {
        var response = GetRemoteResponse(registration.Key);
        var parts = response.Split(' ');
        if (parts[0] != registration.Key)
          return false;

        var expiration = DateTime.ParseExact(parts[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);
        UpdateExpirationDate(registration, expiration);

        return DateTime.Today <= expiration;
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