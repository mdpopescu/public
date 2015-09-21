using System;
using System.Security.Cryptography;
using System.Text;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services.Validators
{
  public class HMACValidator : BaseValidator
  {
    public HMACValidator(string password, Func<LicenseRegistration, object> func, Validator next)
      : base(func, next)
    {
      this.password = password;
    }

    //

    protected override bool InternalIsValid(object obj)
    {
      var details = obj as LicenseRegistration;
      if (details == null)
        return false;

      var s = GetData(details);
      var signature = GetSignature(s);

      return details.Key == signature;
    }

    //

    private readonly string password;

    private static string GetData(LicenseRegistration details)
    {
      var sb = new StringBuilder();

      sb.AppendLine(details.Name);
      sb.AppendLine(details.Contact);
      sb.AppendLine(details.Expiration.ToString(Constants.DATE_FORMAT));

      return sb.ToString();
    }

    private string GetSignature(string s)
    {
      var keyBytes = Encoding.UTF8.GetBytes(password);
      var dataBytes = Encoding.UTF8.GetBytes(s);

      var hmac = new HMACSHA256(keyBytes);
      var hmacBytes = hmac.ComputeHash(dataBytes);

      return Convert.ToBase64String(hmacBytes);
    }
  }
}