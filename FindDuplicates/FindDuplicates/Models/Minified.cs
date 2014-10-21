using System.Drawing;

namespace FindDuplicates.Models
{
  public class Minified
  {
    public string FileName { get; private set; }
    public Image Image { get; private set; }
    public byte[] Hash { get; private set; }

    public Minified(string fileName, Image image, byte[] hash)
    {
      FileName = fileName;
      Image = image;
      Hash = hash;
    }
  }
}