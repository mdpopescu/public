using System.Threading;
using EventSystem.Library.Implementations;
using Tester.Models;

namespace Tester.Implementations
{
    public class TickGenerator
    {
        public TickGenerator()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Timer(_ => EventBus.Publish(new Tick()), null, 5, 5);
        }
    }
}