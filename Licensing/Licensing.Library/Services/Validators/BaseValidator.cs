using System;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services.Validators
{
  public abstract class BaseValidator : Validator
  {
    public bool Isvalid(LicenseRegistration registration)
    {
      if (registration == null)
        return false;

      var valid = InternalIsValid(func(registration));
      return Next == null
        ? valid
        : valid && Next.Isvalid(registration);
    }

    //

    protected Validator Next { get; set; }

    protected abstract bool InternalIsValid(object obj);

    protected BaseValidator(Func<LicenseRegistration, object> func, Validator next = null)
    {
      this.func = func;
      Next = next;
    }

    //

    private readonly Func<LicenseRegistration, object> func;
  }
}