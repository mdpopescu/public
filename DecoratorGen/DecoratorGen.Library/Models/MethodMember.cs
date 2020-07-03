using System.Linq;

namespace DecoratorGen.Library.Models
{
    public class MethodMember : Member
    {
        public string TypeName { get; set; }
        public string Name { get; set; }
        public Argument[] Arguments { get; set; }

        public override string ToString() =>
            @$"public {TypeName} {Name}({ArgsToString1()}) =>
        decorated.{Name}({ArgsToString2()});";

        //

        private string ArgsToString1() =>
            string.Join(", ", Arguments.Select(it => $"{it.TypeName} {it.Name}"));

        private string ArgsToString2() =>
            string.Join(", ", Arguments.Select(it => it.Name));
    }
}