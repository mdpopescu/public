using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using Renfield.Licensing.Library.Contracts;

namespace Renfield.Licensing.Library.Services
{
  public class WinSys : Sys
  {
    public string GetProcessorId()
    {
      var wqlQuery = new WqlObjectQuery("SELECT * FROM Win32_Processor");
      var searcher = new ManagementObjectSearcher(wqlQuery);

      var list = searcher.Get().Cast<ManagementObject>().ToList();
      return list[0].Properties["ProcessorId"].Value.ToString();
    }

    public string Encode(IEnumerable<KeyValuePair<string, string>> pairs)
    {
      var fields = pairs.Select(pair => pair.Key + "=" + Uri.EscapeDataString(pair.Value));
      return string.Join("&", fields);
    }
  }
}