using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Renfield.Spikes
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      // given the viewport (video) size of W x H
      // given the image size of w x h
      // generate a series of N images (= N frames) so that the image is scrolled through the viewport
      // (equivalently, the viewport pans over the image)

      var w = 50;
      var h = 50;
      Console.WriteLine("w x h = {0:0.00} x {1:0.00}", w, h);
      var W = 160;
      var H = 90;
      Console.WriteLine("W x H = {0:0.00} x {1:0.00}", W, H);

      var rw = (double) W / w;
      Console.WriteLine("rw = {0:0.00}", rw);
      var rh = (double) H / h;
      Console.WriteLine("rh = {0:0.00}", rh);

      var r = Math.Max(rw, rh);
      Console.WriteLine("r = {0:0.00}", r);

      var wScaled = r * w;
      Console.WriteLine("wScaled = {0:0.00}", wScaled);
      var hScaled = r * h;
      Console.WriteLine("hScaled = {0:0.00}", hScaled);

      var N = 10;
      Console.WriteLine("Number of frames: {0}", N);

      var Rs = Scroll(W, H, wScaled, hScaled, N).ToList();
      if (!Rs.Any())
        Console.WriteLine("No scrolling");
      else
        foreach (var R in Rs)
          Console.WriteLine(R);
    }

    private static IEnumerable<Rectangle> Scroll(int W, int H, double wScaled, double hScaled, int N)
    {
      var rnd = new Random();

      double step;
      if (hScaled > H)
      {
        // Console.WriteLine("vertical scrolling");

        var x = 0.0;
        var y = 0.0;
        step = (hScaled - H) / (N - 1);
        if (rnd.Next(2) == 0)
        {
          y = hScaled - H;
          step = -step;
        }
        // Console.WriteLine("Step: {0:0.00}", step);

        var R = new Rectangle((int) Math.Truncate(x), (int) Math.Truncate(y), W, H);
        for (var i = 0; i < N; i++)
        {
          yield return R;

          y += step;
          R.Y = (int) Math.Truncate(y);
        }
      }
      else if (wScaled > W)
      {
        //Console.WriteLine("horizontal scrolling");

        var x = 0.0;
        var y = 0.0;
        step = (wScaled - W) / (N - 1);
        if (rnd.Next(2) == 0)
        {
          x = wScaled - W;
          step = -step;
        }
        //Console.WriteLine("Step: {0:0.00}", step);

        var R = new Rectangle((int) Math.Truncate(x), (int) Math.Truncate(y), W, H);
        for (var i = 0; i < N; i++)
        {
          yield return R;

          x += step;
          R.X = (int) Math.Truncate(x);
        }
      }
    }
  }
}