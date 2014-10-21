using System;
using System.Drawing;

namespace FindDuplicates.Contracts
{
  public interface ImageWriter : IDisposable
  {
    void Save(Image image, string path);
  }
}