using System;
using System.Globalization;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class WebResponseParser : ResponseParser
  {
    public RemoteResponse Parse(string response)
    {
      try
      {
        response = ExtractContent(response);
        var parts = response.Split(' ');

        return new RemoteResponse
        {
          Key = parts[0],
          Expiration = DateTime.ParseExact(parts[1], Constants.DATE_FORMAT, CultureInfo.InvariantCulture),
        };
      }
      catch
      {
        return new RemoteResponse();
      }
    }

    private static string ExtractContent(string response)
    {
      const string Q = "\"";

      if (response.StartsWith(Q) && response.EndsWith(Q) && response.Length > 1)
        response = response.Substring(1, response.Length - 2);

      return response;
    }
  }
}