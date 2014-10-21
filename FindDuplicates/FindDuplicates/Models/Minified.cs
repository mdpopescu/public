using System.Drawing;

namespace FindDuplicates.Models
{
  public class Minified
  {
    public string FileName { get; private set; }
    public Image Image { get; private set; }
    public byte[] Bytes { get; private set; }
    public ulong Hash { get; private set; }

    public Minified(string fileName, Image image, byte[] bytes, ulong hash)
    {
      FileName = fileName;
      Image = image;
      Bytes = bytes;
      Hash = hash;
    }
  }
}