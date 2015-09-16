using System.Collections.Generic;

namespace Renfield.Licensing.Library.Contracts
{
  public interface Sys
  {
    string GetProcessorId();
    string Encode(IEnumerable<KeyValuePair<string, string>> fields);
  }
}