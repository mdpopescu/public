using System;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services.Validators
{
  public class ExpirationValidator : BaseValidator
  {
    public ExpirationValidator(Func<LicenseRegistration, object> func, Validator next)
      : base(func, next)
    {
    }

    //

    protected override bool InternalIsValid(object obj)
    {
      return obj is DateTime && DateTime.Today <= (DateTime) obj;
    }
  }
}