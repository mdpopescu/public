using Microsoft.Win32;
using Renfield.Licensing.Library.Contracts;

namespace Renfield.Licensing.Library.Services
{
  public class RegistryIO : StringIO
  {
    public RegistryIO(RegistryKey key)
    {
      this.key = key;
    }

    public string Read()
    {
      return key.GetValue(null) as string;
    }

    public void Write(string s)
    {
      key.SetValue(null, s);
    }

    //

    private readonly RegistryKey key;
  }
}