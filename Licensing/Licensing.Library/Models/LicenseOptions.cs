namespace Renfield.Licensing.Library.Models
{
  public class LicenseOptions
  {
    public string Password { get; set; }
    public string Salt { get; set; }
    public string CheckUrl { get; set; }
    public string Company { get; set; }
    public string Product { get; set; }
  }
}