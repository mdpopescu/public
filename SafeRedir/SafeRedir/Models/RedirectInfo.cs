using System.ComponentModel.DataAnnotations;

namespace Renfield.SafeRedir.Models
{
  public class RedirectInfo
  {
    [Display(Name = "URL to shorten", Prompt = "This will be returned initially")]
    [Required(ErrorMessage = "Please enter the URL.")]
    public string URL { get; set; }

    [Display(Name = "Safe URL (after TTL expires)", Prompt = "This will be returned after the timeout")]
    public string SafeURL { get; set; }

    [Display(Name = "Time-to-live (sec)", Prompt = "Seconds")]
    public int? TTL { get; set; }

    public RedirectInfo()
    {
      SafeURL = Constants.DEFAULT_SAFE_URL;
      TTL = Constants.DEFAULT_TTL;
    }
  }
}