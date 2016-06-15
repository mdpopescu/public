using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;

namespace Renfield.AppUpdater.Tests
{
  public static class ImageExtender
  {
    public static bool IsIdenticalTo(this Image image1, Image image2)
    {
      //Test to see if we have the same size of image

      if (image1.Size != image2.Size)
        return false;

      //Convert each image to a byte array

      var ic = new ImageConverter();
      var btImage1 = new byte[1];
      btImage1 = (byte[]) ic.ConvertTo(image1, btImage1.GetType());
      var btImage2 = new byte[1];
      btImage2 = (byte[]) ic.ConvertTo(image2, btImage2.GetType());

      //Compute a hash for each image

      var shaM = new SHA256Managed();
      var hash1 = shaM.ComputeHash(btImage1);
      var hash2 = shaM.ComputeHash(btImage2);

      //Compare the hash values

      return SameByteArray(hash1, hash2);
    }

    //

    private static bool SameByteArray(IEnumerable<byte> value1, IEnumerable<byte> value2)
    {
      return value1
        .Zip(value2, (b1, b2) => new { b1, b2 })
        .All(bb => bb.b1 == bb.b2);
    }
  }
}