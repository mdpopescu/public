using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services.Validators
{
  public class OrValidator : BaseValidator
  {
    public OrValidator(Validator left, Validator right, Validator next)
      : base(_ => _, next)
    {
      this.left = left;
      this.right = right;
    }

    //

    protected override bool InternalIsValid(object obj)
    {
      var details = obj as LicenseRegistration;
      if (details == null)
        return false;

      return left.Isvalid(details) || right.Isvalid(details);
    }

    //

    private readonly Validator left;
    private readonly Validator right;
  }
}