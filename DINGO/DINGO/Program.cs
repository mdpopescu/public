namespace DINGO
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // input: project file
            // project file syntax:
            //   solution=<full path of .sln file>
            //   OR
            //   project=<full path of .csproj file>
            //   OR
            //   file=<full path of .cs file>
            //   ---
            //   ref=<full path of DINGO project file which defines referenced DLLs>
            //   ---
            //   output=<path where the generated files will be saved>

            // algorithm:
            // PARSE: identify and load all source files
            // PARSE: identify all types (a file can contain multiple types - classes, interfaces, structs, and enums - and a class can be spread over multiple files)
            // PARSE: generate and save an AST for the class
            // COMPILE: convert each AST to HTML
            // LINK: fix references as needed
            // LINK: create helper files (e.g., a .css file)
        }
    }
}