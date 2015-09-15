using System;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services.Validators
{
  public abstract class BaseValidator : Validator
  {
    public bool Isvalid(LicenseRegistration registration)
    {
      var valid = InternalIsValid(func(registration));
      return next == null
        ? valid
        : valid && next.Isvalid(registration);
    }

    //

    protected abstract bool InternalIsValid(object obj);

    protected BaseValidator(Func<LicenseRegistration, object> func, Validator next)
    {
      this.func = func;
      this.next = next;
    }

    //

    private readonly Func<LicenseRegistration, object> func;
    private readonly Validator next;
  }
}