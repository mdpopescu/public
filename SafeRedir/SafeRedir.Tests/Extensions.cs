using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Renfield.SafeRedir.Tests
{
  public static class Extensions
  {
    public static void ValidateModel(this Controller controller, object model)
    {
      var validationContext = new ValidationContext(model, null, null);
      var validationResults = new List<ValidationResult>();

      Validator.TryValidateObject(model, validationContext, validationResults, true);
      foreach (var validationResult in validationResults)
        controller.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
    }
  }
}