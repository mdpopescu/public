using System;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Tests
{
  public static class ObjectMother
  {
    public const string KEY = "{D98F6376-94F7-4D82-AA37-FC00F0166700}";
    public const string NAME = "Marcel";
    public const string CONTACT = "mdpopescu@gmail.com";

    public static DateTime OldDate
    {
      get { return new DateTime(2000, 1, 2); }
    }

    public static DateTime NewDate
    {
      get { return new DateTime(2001, 2, 3); }
    }

    public static DateTime FutureDate
    {
      get { return new DateTime(9999, 12, 31); }
    }

    public static LicenseRegistration CreateRegistration()
    {
      return new LicenseRegistration
      {
        CreatedOn = OldDate,
        Limits = new Limits
        {
          Days = -1,
          Runs = -1,
        },
        Key = KEY,
        Name = NAME,
        Contact = CONTACT,
        Expiration = FutureDate,
      };
    }
  }
}