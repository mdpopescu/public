﻿namespace BigDataProcessing.Library.Models
{
  public class Configuration
  {
    /// <summary>
    ///   The data source (e.g., a stream).
    /// </summary>
    public object Input { get; set; }

    /// <summary>
    ///   The output (e.g., a stream).
    /// </summary>
    public object Output { get; set; }

    /// <summary>
    ///   The number of threads used to process the input after it has been read.
    /// </summary>
    public int Threads { get; set; }

    public Configuration()
    {
      Threads = 1;
    }
  }
}