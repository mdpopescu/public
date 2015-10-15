using System;
using System.Linq;
using System.Threading.Tasks;

namespace Propagators
{
  public class Propagator
  {
    public Propagator(Func<object[], object> func)
    {
      this.func = func;
    }

    public void Connect(Cell[] inputs, Cell output)
    {
      this.inputs = inputs ?? new Cell[0];
      this.output = output;

      foreach (var input in this.inputs)
        input.ValueChanged += async (_, __) => await ApplyAsync();

      // try applying immediately, in case the inputs already have values or there are no inputs
      ApplyAsync().Wait();
    }

    //

    private readonly Func<object[], object> func;

    private Cell[] inputs;
    private Cell output;

    private async Task ApplyAsync()
    {
      var inputValues = inputs.Select(it => it.Value).ToArray();
      if (inputValues.All(it => it != null))
        await output.SetValueAsync(func(inputValues));
    }
  }
}