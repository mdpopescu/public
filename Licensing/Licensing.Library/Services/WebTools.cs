using System;
using System.Collections.Generic;
using System.Linq;

namespace Renfield.Licensing.Library.Services
{
  public static class WebTools
  {
    public static string FormUrlEncoded(IEnumerable<KeyValuePair<string, string>> pairs)
    {
      var fields = pairs.Select(pair => pair.Key + "=" + Uri.EscapeDataString(pair.Value));
      return string.Join("&", fields);
    }
  }
}