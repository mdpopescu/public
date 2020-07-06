using System;
using System.Linq;
using DecoratorGen.Library.Contracts;

namespace DecoratorGen.Library.Services
{
    public class CodeGenerator : ICodeGenerator
    {
        public CodeGenerator(IParser parser, IMemberGenerator memberGenerator, IClassNameGenerator classNameGenerator)
        {
            this.parser = parser;
            this.memberGenerator = memberGenerator;
            this.classNameGenerator = classNameGenerator;
        }

        public string Generate(string code)
        {
            var interfaceData = parser.ExtractInterface(code);
            var className = classNameGenerator.GenerateClassName(interfaceData.Code);
            var members = parser.ExtractMembers(interfaceData.Code);
            var membersCode = members.Select(memberGenerator.Generate);
            var membersLines = string.Join(Environment.NewLine, membersCode.Select(it => "    " + it));

            return $@"public class {className} : {interfaceData.Name}
{{
    public {className}({interfaceData.Name} decorated)
    {{
        this.decorated = decorated;
    }}

{membersLines}

    //

    private readonly {interfaceData.Name} decorated;
}}";
        }

        //

        private readonly IParser parser;
        private readonly IMemberGenerator memberGenerator;
        private readonly IClassNameGenerator classNameGenerator;
    }
}