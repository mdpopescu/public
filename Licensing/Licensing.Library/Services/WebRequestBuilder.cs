using System.Collections.Generic;
using System.Linq;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class WebRequestBuilder : RequestBuilder
  {
    public WebRequestBuilder(Sys sys, string queryTemplate)
    {
      this.sys = sys;
      this.queryTemplate = queryTemplate;
    }

    public string BuildQuery(LicenseRegistration registration)
    {
      return queryTemplate
        .Replace("{key}", registration.Key)
        .Replace("{pid}", sys.GetProcessorId());
    }

    public string BuildData(LicenseRegistration registration)
    {
      var fields = registration.GetLicenseFields().ToList();
      fields.Add(new KeyValuePair<string, string>("ProcessorId", sys.GetProcessorId()));

      return sys.Encode(fields);
    }

    //

    private readonly Sys sys;
    private readonly string queryTemplate;
  }
}