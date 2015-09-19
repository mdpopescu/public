using System;
using System.Collections.Generic;
using System.Linq;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class RemoteCheckerClient : RemoteChecker
  {
    public RemoteCheckerClient(Sys sys, Remote remote, ResponseParser parser)
    {
      this.sys = sys;
      this.remote = remote;
      this.parser = parser;
    }

    public void Check(LicenseRegistration registration)
    {
      var query = BuildQuery(registration, sys.GetProcessorId());
      var response = remote.Get(query);

      var expiration = GetExpirationDate(registration, response);
      registration.Expiration = expiration.GetValueOrDefault();
    }

    public void Submit(LicenseRegistration registration)
    {
      var data = BuildData(registration, sys.GetProcessorId());
      remote.Post(data);
    }

    //

    private readonly Sys sys;
    private readonly Remote remote;
    private readonly ResponseParser parser;

    private static string BuildQuery(LicenseRegistration registration, string processorId)
    {
      return string.Format("Key={0}&ProcessorId={1}", registration.Key, processorId);
    }

    private string BuildData(LicenseRegistration registration, string processorId)
    {
      var fields = registration.GetLicenseFields().ToList();
      fields.Add(new KeyValuePair<string, string>("ProcessorId", processorId));

      return sys.Encode(fields);
    }

    private DateTime? GetExpirationDate(LicenseRegistration registration, string response)
    {
      var parsed = parser.Parse(response);
      return parsed != null && parsed.Key == registration.Key
        ? parsed.Expiration
        : (DateTime?) null;
    }
  }
}