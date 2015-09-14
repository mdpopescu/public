using System;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services.Validators
{
  public class ExpirationValidator : BaseValidator
  {
    public ExpirationValidator(Validator next)
      : base(next)
    {
    }

    protected override bool InternalIsValid(LicenseRegistration registration)
    {
      return DateTime.Today <= registration.Expiration;
    }
  }
}