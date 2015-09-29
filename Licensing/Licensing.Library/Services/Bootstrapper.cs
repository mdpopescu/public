using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Extensions;
using Renfield.Licensing.Library.Models;
using Renfield.Licensing.Library.Services.Validators;

namespace Renfield.Licensing.Library.Services
{
  public static class Bootstrapper
  {
    public static Storage GetStorage(LicenseOptions options)
    {
      var details = LoadCompanyAndProduct(options);

      var pathBuilder = new RegistryPathBuilder();
      var subkey = pathBuilder.GetPath(details);
      var key = Registry.CurrentUser.OpenSubKey(subkey, RegistryKeyPermissionCheck.ReadWriteSubTree)
                ?? Registry.CurrentUser.CreateSubKey(subkey, RegistryKeyPermissionCheck.ReadWriteSubTree);
      var io = new RegistryIO(key);

      var encryptor = GetEncryptor(options);
      var serializer = new LicenseSerializer();

      return new SecureStorage(io, encryptor, serializer);
    }

    public static LicenseChecker GetLicenseChecker(LicenseOptions options)
    {
      var sys = new WinSys();
      var remote = GetRemoteChecker(options, sys);
      var validator = GetValidator(options);

      return new LocalChecker(remote, validator);
    }

    //

    private static Details LoadCompanyAndProduct(LicenseOptions options)
    {
      var r1 = new OptionsDetailsReader(options);
      var r2 = new AssemblyDetailsReader(GetRootAssembly());
      var reader = new CompositeDetailsReader(r1, r2);

      return reader.Read();
    }

    private static Assembly GetRootAssembly()
    {
      // ReSharper disable once AssignNullToNotNullAttribute
      return Assembly.GetEntryAssembly()
             ?? new StackTrace().GetFrames().Last().GetMethod().Module.Assembly;
    }

    private static Encryptor GetEncryptor(LicenseOptions options)
    {
      return String.IsNullOrWhiteSpace(options.Password) || String.IsNullOrWhiteSpace(options.Salt)
        ? (Encryptor) new NullEncryptor()
        : new RijndaelEncryptor(options.Password, options.Salt);
    }

    private static LicenseChecker GetRemoteChecker(LicenseOptions options, Sys sys)
    {
      var checkUrl = options.CheckUrl;
      if (String.IsNullOrWhiteSpace(checkUrl))
        return new NullRemoteChecker();

      var submitUrl = options.SubmitUrl.NullIfEmpty() ?? checkUrl;
      var remote = new WebRemote(submitUrl);
      var builder = new WebRequestBuilder(sys, checkUrl);
      var parser = new WebResponseParser();

      return new RemoteCheckerClient(remote, builder, parser);
    }

    private static Validator GetValidator(LicenseOptions options)
    {
      return new OrValidator(
        new HMACValidator(options.Password, it => it, null),
        new GuidValidator(it => it.Key, null),
        new NonEmptyValidator(it => it.Name,
          new NonEmptyValidator(it => it.Contact,
            new ExpirationValidator(it => it.Expiration,
              null))));
    }
  }
}