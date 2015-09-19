using System;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Tests
{
  public static class ObjectMother
  {
    public const string KEY = "{D98F6376-94F7-4D82-AA37-FC00F0166700}";

    public static DateTime OldDate
    {
      get { return new DateTime(2000, 1, 2); }
    }

    public static DateTime NewDate
    {
      get { return new DateTime(2001, 2, 3); }
    }

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
        Key = KEY,
        Name = "Marcel",
        Contact = "mdpopescu@gmail.com",
        ProcessorId = "1",
        Expiration = new DateTime(9999, 12, 31),
      };
    }
  }
}