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

      var query = BuildQuery(registration, processorId);
      var response = remote.Get(query);

      return GetExpirationDate(registration, response);
    }

    public void Submit(LicenseRegistration registration)
    {
      var processorId = sys.GetProcessorId();
      registration.ProcessorId = processorId;

      var data = BuildData(registration);
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

    private string BuildData(LicenseRegistration registration)
    {
      var fields = registration.GetLicenseFields();
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