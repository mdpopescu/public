namespace Renfield.Licensing.Library.Contracts
{
  public interface StringIO
  {
    string Read();
    void Write(string s);
  }
}