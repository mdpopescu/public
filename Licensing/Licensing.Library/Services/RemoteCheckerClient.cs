using System;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class RemoteCheckerClient : RemoteChecker
  {
    public RemoteCheckerClient(Remote remote, RequestBuilder builder, ResponseParser parser)
    {
      this.remote = remote;
      this.builder = builder;
      this.parser = parser;
    }

    public void Check(LicenseRegistration registration)
    {
      var query = builder.BuildQuery(registration);
      var response = remote.Get(query);

      var expiration = GetExpirationDate(registration, response);
      registration.Expiration = expiration.GetValueOrDefault();
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