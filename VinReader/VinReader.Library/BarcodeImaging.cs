using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Renfield.VinReader.Library
{
  // adapted from http://www.codeproject.com/Articles/10734/Reading-Barcodes-from-an-Image-II
  public class BarcodeImaging
  {
    public static void FullScanPageCode39(ref ArrayList codesRead, Bitmap bmp, int numscans)
    {
      for (var i = 0; i < 4; i++)
      {
        bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
        VScanPageCode39(ref codesRead, bmp, numscans);
      }
    }

    public static void VScanPageCode39(ref ArrayList codesRead, Bitmap bmp, int numscans)
    {
      for (var i = 0; i < numscans; i++)
      {
        var read = ReadCode39(bmp, i * (bmp.Height / numscans), (i * (bmp.Height / numscans)) + (bmp.Height / numscans));

        var stringCodes = codesRead.Cast<object>().Select(o => o.ToString());
        var test = read.Trim() == "" || stringCodes.Any(it => it == read);
        if (!(test))
          codesRead.Add(read);
      }
    }

    public static string ReadCode39(Bitmap bmp, int startheight, int endheight)
    {
      // To find a horizontal barcode, find the vertical histogram to find individual barcodes, 
      // then get the vertical histogram to decode each
      var vertHist = VerticalHistogram(bmp, startheight, endheight);

      // Set the threshold for determining dark/light bars to half way between the histograms min/max
      var threshold = vertHist.min + ((vertHist.max - vertHist.min) / 2);

      // Variables needed to check for
      var patternString = "";
      var nBarStart = -1;
      var nNarrowBarWidth = -1;
      var bDarkBar = false;

      // Find the narrow and wide bars
      for (var i = 0; i < vertHist.histogram.Length; ++i)
      {
        // First find the narrow bar width
        if (nNarrowBarWidth < 0)
        {
          if (nBarStart < 0)
          {
            // The code doesn't start until we see a dark bar
            if (vertHist.histogram[i] <= threshold)
            {
              // We detected a dark bar, save it's start position
              nBarStart = i;
            }
          }
          else
          {
            if (vertHist.histogram[i] > threshold)
            {
              // We detected the end of first the dark bar, save the narrow bar width and 
              // start the rest of the barcode  
              nNarrowBarWidth = i - nBarStart + 1;
              patternString += "n";
              nBarStart = i;
              bDarkBar = false;
            }
          }
        }
        else
        {
          if (bDarkBar)
          {
            // We're on a dark bar, detect when the bar becomes light again
            if (vertHist.histogram[i] > threshold)
            {
              var nBarWidth = i - nBarStart;
              if (nBarWidth > nNarrowBarWidth * WIDEFACTOR)
              {
                // The light bar was wider than the narrow bar width, it's a wide bar
                patternString += "w";
                nBarStart = i;
              }
              else
              {
                // The light bar is a narrow bar
                patternString += "n";
                nBarStart = i;
              }
              bDarkBar = false;
            }
          }
          else
          {
            // We're on a light bar, detect when the bar becomes dark
            if (vertHist.histogram[i] <= threshold)
            {
              if ((i - nBarStart) > (nNarrowBarWidth))
              {
                // The dark bar was wider than the narrow bar width, it's a wide bar
                patternString += "w";
                nBarStart = i;
              }
              else
              {
                // The dark bar is a narrow bar
                patternString += "n";
                nBarStart = i;
              }
              bDarkBar = true;
            }
          }
        }
      }

      // We now have a barcode in terms of narrow & wide bars... Parse it!
      var dataString = "";

      // Each pattern within code 39 is nine bars with one white bar between each pattern
      var index = 0;
      if (patternString.Length > 9)
      {
        while (index < patternString.Length - 8)
        {
          // Create an array of charachters to hold the pattern to be tested
          var pattern = new char[9];
          // Stuff the pattern with data from the pattern string
          patternString.CopyTo(index, pattern, 0, 9);

          var somedata = ParsePattern(new string(pattern));
          dataString += somedata;

          if (somedata == null)
          {
            index++;
          }
          else
          {
            index += 10;
          }
        }
      }

      return dataString;
    }

    //

    private const float WIDEFACTOR = 2.0f; // minimum width of wide bar compared to narrow bar

    /// <summary>
    /// Vertical histogram of an image
    /// </summary>
    /// <param name="bmp">the bitmap to be processed</param>
    /// <param name="startheight"></param>
    /// <param name="endheight"></param>
    /// <returns>a histogramResult representing the vertical histogram</returns>
    private static HistogramResult VerticalHistogram(Bitmap bmp, int startheight, int endheight)
    {
      // Create the return value
      var histResult = new float[bmp.Width];

      var vertSum = new float[bmp.Width];
      // Start the max value at zero
      float maxValue = 0;
      // Start the min value at the absolute maximum
      float minValue = 255;

      // GDI+ still lies to us - the return format is BGR, NOT RGB.
      var bmData = bmp.LockBits(new Rectangle(0, startheight, bmp.Width, endheight - startheight),
        ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

      var stride = bmData.Stride;
      var scan0 = bmData.Scan0;

      unsafe
      {
        var p = (byte*) (void*) scan0;

        var nOffset = stride - bmp.Width * 3;

        for (var y = startheight; y < endheight; ++y)
        {
          for (var x = 0; x < bmp.Width; ++x)
          {
            // Add up all the pixel values vertically (average the R,G,B channels)
            vertSum[x] += ((p[0] + (p + 1)[0] + (p + 2)[0]) / 3.0f);

            p += 3;
          }
          p += nOffset;
        }
      }

      bmp.UnlockBits(bmData);

      // Now get the average of the row by dividing the pixel by num pixels
      for (var i = 0; i < bmp.Width; i++)
      {
        histResult[i] = (vertSum[i] / (endheight - startheight));
        //Save the max value for later
        if (histResult[i] > maxValue)
          maxValue = histResult[i];
        // Save the min value for later
        if (histResult[i] < minValue)
          minValue = histResult[i];
      }

      return new HistogramResult {histogram = histResult, max = maxValue, min = minValue};
    }

    private static string ParsePattern(string pattern)
    {
      switch (pattern)
      {
        case "wnnwnnnnw":
          return "1";
        case "nnwwnnnnw":
          return "2";
        case "wnwwnnnnn":
          return "3";
        case "nnnwwnnnw":
          return "4";
        case "wnnwwnnnn":
          return "5";
        case "nnwwwnnnn":
          return "6";
        case "nnnwnnwnw":
          return "7";
        case "wnnwnnwnn":
          return "8";
        case "nnwwnnwnn":
          return "9";
        case "nnnwwnwnn":
          return "0";
        case "wnnnnwnnw":
          return "A";
        case "nnwnnwnnw":
          return "B";
        case "wnwnnwnnn":
          return "C";
        case "nnnnwwnnw":
          return "D";
        case "wnnnwwnnn":
          return "E";
        case "nnwnwwnnn":
          return "F";
        case "nnnnnwwnw":
          return "G";
        case "wnnnnwwnn":
          return "H";
        case "nnwnnwwnn":
          return "I";
        case "nnnnwwwnn":
          return "J";
        case "wnnnnnnww":
          return "K";
        case "nnwnnnnww":
          return "L";
        case "wnwnnnnwn":
          return "M";
        case "nnnnwnnww":
          return "N";
        case "wnnnwnnwn":
          return "O";
        case "nnwnwnnwn":
          return "P";
        case "nnnnnnwww":
          return "Q";
        case "wnnnnnwwn":
          return "R";
        case "nnwnnnwwn":
          return "S";
        case "nnnnwnwwn":
          return "T";
        case "wwnnnnnnw":
          return "U";
        case "nwwnnnnnw":
          return "V";
        case "wwwnnnnnn":
          return "W";
        case "nwnnwnnnw":
          return "X";
        case "wwnnwnnnn":
          return "Y";
        case "nwwnwnnnn":
          return "Z";
        case "nwnnnnwnw":
          return "-";
        case "wwnnnnwnn":
          return ".";
        case "nwwnnnwnn":
          return " ";
        case "nwnnwnwnn":
          return "*";
        case "nwnwnwnnn":
          return "$";
        case "nwnwnnnwn":
          return "/";
        case "nwnnnwnwn":
          return "+";
        case "nnnwnwnwn":
          return "%";
        default:
          return null;
      }
    }

    /// <summary>
    /// Structure used to return the processed data from an image's histogram
    /// </summary>
    private struct HistogramResult
    {
      public float[] histogram;
      public float min;
      public float max;
    }
  }
}