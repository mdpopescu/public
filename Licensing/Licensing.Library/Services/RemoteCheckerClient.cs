using System;
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

    public DateTime? Check(LicenseRegistration registration)
    {
      var processorId = sys.GetProcessorId();
      registration.ProcessorId = processorId;

      var response = remote.Get(BuildQuery(registration, processorId));
      return GetExpirationDate(registration, response);
    }

    public DateTime? Submit(LicenseRegistration registration)
    {
      var processorId = sys.GetProcessorId();
      registration.ProcessorId = processorId;

      var fields = registration.GetLicenseFields();
      var data = sys.Encode(fields);

      var response = remote.Post(data);
      return GetExpirationDate(registration, response);
    }

    //

    private readonly Sys sys;
    private readonly Remote remote;
    private readonly ResponseParser parser;

    private string BuildQuery(LicenseRegistration registration, string processorId)
    {
      return string.Format("Key={0}&ProcessorId={1}", registration.Key, processorId);
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