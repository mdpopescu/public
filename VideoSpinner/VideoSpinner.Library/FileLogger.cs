using System;
using System.IO;

namespace Renfield.VideoSpinner.Library
{
  public class FileLogger : Logger
  {
    public FileLogger(string fileName)
    {
      this.fileName = fileName;
    }

    public void Log(string message)
    {
      File.AppendAllText(fileName, message + Environment.NewLine);
    }

    //

    private readonly string fileName;
  }
}