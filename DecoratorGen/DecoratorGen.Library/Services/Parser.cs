using System.Collections.Generic;
using System.Linq;
using DecoratorGen.Library.Contracts;
using DecoratorGen.Library.Models;

namespace DecoratorGen.Library.Services
{
    public class Parser : IParser
    {
        public InterfaceData ExtractInterface(string code) => new InterfaceData();
        public IEnumerable<Member> ExtractMembers(string code) => Enumerable.Empty<Member>();
    }
}