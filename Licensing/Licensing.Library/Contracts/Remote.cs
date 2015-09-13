namespace Renfield.Licensing.Library.Contracts
{
  public interface Remote
  {
    string Get(string query);
    string Post(string data);
  }
}