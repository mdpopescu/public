using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services.Validators
{
  public class ProcessorIdValidator : BaseValidator
  {
    public ProcessorIdValidator(Validator next)
      : base(next)
    {
    }

    protected override bool InternalIsValid(LicenseRegistration registration)
    {
      return !string.IsNullOrWhiteSpace(registration.ProcessorId);
    }
  }
}