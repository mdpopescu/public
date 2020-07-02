using System.Collections.Generic;
using DecoratorGen.Library.Models;

namespace DecoratorGen.Library.Contracts
{
    public interface IParser
    {
        InterfaceData ExtractInterface(string code);
        IEnumerable<Member> ExtractMembers(string code);
    }
}