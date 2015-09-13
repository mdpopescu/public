using System;
using System.Globalization;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class ResponseParserImpl : ResponseParser
  {
    public RemoteResponse Parse(string response)
    {
      try
      {
        var parts = response.Split(' ');

        return new RemoteResponse
        {
          Key = parts[0],
          Expiration = DateTime.ParseExact(parts[1], "yyyy-MM-dd", CultureInfo.InvariantCulture),
        };
      }
      catch
      {
        return new RemoteResponse();
      }
    }
  }
}