namespace Renfield.PageFaults
{
  public interface Cache
  {
    int CacheSize { get; }
    int?[] Pages { get; }
    int PageFaults { get; }
    
    void AddPage(int pageNo);
  }
}