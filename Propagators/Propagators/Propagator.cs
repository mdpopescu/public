using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Propagators
{
  public class Propagator
  {
    public Propagator(Func<object[], Task<object>> func)
    {
      this.func = func;
    }

    public Propagator(Func<object[], object> func)
    {
      this.func = arr => Task.FromResult(func(arr));
    }

    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    public void Connect(Cell output, params Cell[] inputs)
    {
      this.inputs = inputs ?? new Cell[0];
      this.output = output;

      foreach (var input in this.inputs)
        input.ValueChanged += async (_, __) => await ApplyAsync();

      // try applying immediately, in case the inputs already have values or there are no inputs
      ApplyAsync().Wait();
    }

    //

    private readonly Func<object[], Task<object>> func;

    private Cell[] inputs;
    private Cell output;

    private async Task ApplyAsync()
    {
      var inputValues = inputs.Select(it => it.Value).ToArray();
      if (inputValues.All(it => it != null))
        output.SetValue(await func(inputValues));
    }
  }
}