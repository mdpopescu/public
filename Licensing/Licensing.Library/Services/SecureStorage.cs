using System;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class SecureStorage : Storage
  {
    public LicenseRegistration Load()
    {
      throw new NotImplementedException();
    }

    public void Save(LicenseRegistration registration)
    {
      throw new NotImplementedException();
    }
  }
}