using System.Drawing;

namespace FindDuplicates.Models
{
  public class Minified
  {
    public string FileName { get; private set; }
    public Image Image { get; private set; }
    public ulong Hash { get; private set; }

    public Minified(string fileName, Image image, ulong hash)
    {
      FileName = fileName;
      Image = image;
      Hash = hash;
    }
  }
}