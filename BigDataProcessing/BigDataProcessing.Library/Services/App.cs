using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
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
      var inputSplitter = new RxGenericSplitter<string>();
      var multiReader = new RxMultiReader<object, string>(reader, inputSplitter);

      var sources = multiReader.Read(config.Input, config.Threads, Scheduler.Default);
      if (sources == null)
      {
        logger.Log("Error reading from the input.");
        return;
      }

      var results = sources
        .Select(it => it.ObserveOn(NewThreadScheduler.Default))
        .Select(ProcessSource);

      foreach (var result in results)
        writer.Write(config.Output, result);

      //var tasks = sources
      //  .Select(source => Task.Run(() => ProcessSource(source)))
      //  .ToArray();

      //foreach (var task in tasks)
      //{
      //  task.Wait();
      //  writer.Write(config.Output, task.Result);
      //}
    }

    //

    private readonly Logger logger;
    private readonly RxTextReader reader;
    private readonly RxTextWriter writer;
    private readonly LineConverter[] processors;

    private IObservable<string> ProcessSource(IObservable<string> source)
    {
      return source
        .Select(Process)
        .Where(it => it != null);
    }

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