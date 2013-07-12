namespace Renfield.PageFaults
{
  public interface Cache
  {
    int?[] Pages { get; }
    int PageFaults { get; }
    
    void AddPage(int pageNo);
  }
}