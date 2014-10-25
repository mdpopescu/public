using System.Drawing;

namespace FindDuplicates
{
  public static class ImageExtensions
  {
    public static byte[] ToByteArray(this Bitmap image)
    {
      var converter = new ImageConverter();
      return converter.ConvertTo(image, typeof (byte[])) as byte[];
    }

    public static Bitmap ToBitmap(this byte[] buffer)
    {
      var converter = new ImageConverter();
      return converter.ConvertFrom(buffer) as Bitmap;
    }
  }
}