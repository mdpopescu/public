using System;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services.Validators
{
  public class GuidValidator : BaseValidator
  {
    public GuidValidator(Func<LicenseRegistration, object> func, Validator next)
      : base(func, next)
    {
    }

    //

    protected override bool InternalIsValid(object obj)
    {
      Guid guid;
      return Guid.TryParse(obj + "", out guid);
    }
  }
}