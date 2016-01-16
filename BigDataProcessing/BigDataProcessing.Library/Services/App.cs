using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using BigDataProcessing.Library.Contracts;
using BigDataProcessing.Library.Models;

namespace BigDataProcessing.Library.Services
{
  public class App
  {
    public App(Logger logger, RxTextReader reader, RxTextWriter writer, IEnumerable<LineConverter> processors)
    {
      this.logger = logger;
      this.reader = reader;
      this.writer = writer;
      this.processors = processors.ToArray();
    }

    public void Run(Configuration config)
    {
      var source = reader.Read(config.Input);
      if (source == null)
      {
        logger.Log("Error reading from the input.");
        return;
      }

      var results = source
        .Select(Process)
        .Where(it => it != null);

      writer.Write(config.Output, results);
    }

    //

    private readonly Logger logger;
    private readonly RxTextReader reader;
    private readonly RxTextWriter writer;
    private readonly LineConverter[] processors;

    private string Process(string line)
    {
      var result = line;
      foreach (var processor in processors)
      {
        if (result == null)
          return null;

        result = TryConvert(processor, result);
      }

      return result;
    }

    private static string TryConvert(LineConverter processor, string result)
    {
      try
      {
        return processor.Convert(result);
      }
      catch
      {
        return null;
      }
    }
  }
}