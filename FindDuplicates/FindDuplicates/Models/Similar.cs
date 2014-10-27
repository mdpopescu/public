namespace FindDuplicates.Models
{
  public class Similar
  {
    public string FileName { get; private set; }
    public int Distance { get; private set; }

    public Similar(string fileName, int distance)
    {
      FileName = fileName;
      Distance = distance;
    }
  }
}