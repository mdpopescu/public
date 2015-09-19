using System;
using System.Reflection;
using Microsoft.Win32;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;
using Renfield.Licensing.Library.Services.Validators;

namespace Renfield.Licensing.Library.Services
{
  public static class Bootstrapper
  {
    public static Details LoadRegistration(LicenseOptions options)
    {
      var r1 = new OptionsDetailsReader(options);
      var r2 = new AssemblyDetailsReader(Assembly.GetEntryAssembly());
      var reader = new CompositeDetailsReader(r1, r2);

      return reader.Read();
    }

    public static Storage GetStorage(LicenseOptions options, Details details)
    {
      var pathBuilder = new RegistryPathBuilder();
      var subkey = pathBuilder.GetPath(details.Company, details.Product);
      var key = Registry.CurrentUser.OpenSubKey(subkey, RegistryKeyPermissionCheck.ReadWriteSubTree)
                ?? Registry.CurrentUser.CreateSubKey(subkey, RegistryKeyPermissionCheck.ReadWriteSubTree);
      var io = new RegistryIO(key);

      var encryptor = GetEncryptor(options);
      var serializer = new LicenseSerializer();

      return new SecureStorage(io, encryptor, serializer);
    }

    public static RemoteChecker GetChecker(LicenseOptions options, Sys sys)
    {
      if (String.IsNullOrWhiteSpace(options.CheckUrl))
        return new NullRemoteChecker();

      var remote = new WebRemote("https://" + options.CheckUrl);
      var parser = new ResponseParserImpl();

      return new RemoteCheckerClient(sys, remote, parser);
    }

    public static Validator GetValidator()
    {
      return new GuidValidator(it => it.Key,
        new NonEmptyValidator(it => it.Name,
          new NonEmptyValidator(it => it.Contact,
            new NonEmptyValidator(it => it.ProcessorId,
              new ExpirationValidator(it => it.Expiration,
                null)))));
    }

    //

    private static Encryptor GetEncryptor(LicenseOptions options)
    {
      return String.IsNullOrWhiteSpace(options.Password) || String.IsNullOrWhiteSpace(options.Salt)
        ? (Encryptor) new NullEncryptor()
        : new RijndaelEncryptor(options.Password, options.Salt);
    }
  }
}