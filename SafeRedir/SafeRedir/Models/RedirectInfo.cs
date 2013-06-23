namespace Renfield.SafeRedir.Models
{
  public class RedirectInfo
  {
    public string URL { get; set; }
    public string SafeURL { get; set; }
    public int ExpiresInMins { get; set; }

    public RedirectInfo()
    {
      URL = "";
      SafeURL = "http://www.randomkittengenerator.com/";
      ExpiresInMins = 5;
    }
  }
}