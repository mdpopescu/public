using System;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class RemoteCheckerClient : LicenseChecker
  {
    public RemoteCheckerClient(Remote remote, RequestBuilder builder, ResponseParser parser)
    {
      this.remote = remote;
      this.builder = builder;
      this.parser = parser;
    }

    public LicenseStatus Check(LicenseRegistration registration)
    {
      var query = builder.BuildQuery(registration);
      var response = remote.Get(query);

      var expiration = GetExpirationDate(registration, response);
      if (expiration.HasValue)
        registration.Expiration = expiration.Value;

      return new LicenseStatus
      {
        IsLicensed = expiration.HasValue && DateTime.Today <= registration.Expiration,
        IsTrial = true,
      };
    }

    public void Submit(LicenseRegistration registration)
    {
      var data = builder.BuildData(registration);
      remote.Post(data);
    }

    //

    private readonly Remote remote;
    private readonly RequestBuilder builder;
    private readonly ResponseParser parser;

    private DateTime? GetExpirationDate(LicenseRegistration registration, string response)
    {
      var parsed = parser.Parse(response);
      return parsed != null && parsed.Key == registration.Key
        ? parsed.Expiration
        : (DateTime?) null;
    }
  }
}