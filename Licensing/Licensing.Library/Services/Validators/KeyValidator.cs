using System;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services.Validators
{
  public class KeyValidator : BaseValidator
  {
    public KeyValidator(Validator next)
      : base(next)
    {
    }

    protected override bool InternalIsValid(LicenseRegistration registration)
    {
      Guid guid;
      return Guid.TryParse(registration.Key + "", out guid);
    }
  }
}