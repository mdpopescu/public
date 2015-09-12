using System;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Tests
{
  public static class ObjectMother
  {
    public static LicenseRegistration CreateRegistration()
    {
      return new LicenseRegistration
      {
        CreatedOn = new DateTime(2000, 1, 1),
        Limits = new Limits
        {
          Days = -1,
          Runs = -1,
        },
        Key = "{D98F6376-94F7-4D82-AA37-FC00F0166700}",
        Name = "Marcel",
        Contact = "mdpopescu@gmail.com",
        Expiration = new DateTime(9999, 12, 31),
      };
    }
  }
}