namespace Renfield.SafeRedir.Models
{
  public class RedirectInfo
  {
    public string URL { get; set; }
    public string SafeURL { get; set; }
    public int TTL { get; set; }

    public RedirectInfo()
    {
      URL = "";
      SafeURL = "http://www.randomkittengenerator.com/";
      TTL = 5 * 60;
    }
  }
}