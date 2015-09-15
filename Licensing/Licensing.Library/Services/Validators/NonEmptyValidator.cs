using System;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services.Validators
{
  public class NonEmptyValidator : BaseValidator
  {
    public NonEmptyValidator(Func<LicenseRegistration, object> func, Validator next)
      : base(func, next)
    {
    }

    protected override bool InternalIsValid(object obj)
    {
      return !string.IsNullOrWhiteSpace(obj as string);
    }
  }
}