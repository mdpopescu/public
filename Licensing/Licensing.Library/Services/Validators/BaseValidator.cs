using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services.Validators
{
  public abstract class BaseValidator : Validator
  {
    public bool Isvalid(LicenseRegistration registration)
    {
      var valid = InternalIsValid(registration);
      return next == null
        ? valid
        : next.Isvalid(registration);
    }

    //

    protected abstract bool InternalIsValid(LicenseRegistration registration);

    protected BaseValidator(Validator next)
    {
      this.next = next;
    }

    //

    private readonly Validator next;
  }
}