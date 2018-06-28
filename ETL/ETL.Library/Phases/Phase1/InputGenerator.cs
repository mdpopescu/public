using System;
using System.Collections.Generic;
using System.Linq;

namespace ETL.Library.Phases.Phase1
{
    public class InputGenerator
    {
        public IEnumerable<string> Process(InputSpec spec)
        {
            return new[] { GenerateRecord(spec), GenerateExtensions() };
        }

        //

        private const string RECORD_TEMPLATE = @"public class Record
{{
    public static Record Read(TextReader reader)
    {{
        return new Record
        {{
{0}
        }};
    }}

{1}
}}
";

        private const string READ_TEMPLATE = "            {0} = reader.ReadString({1}),";
        private const string FIELD_TEMPLATE = "    public string {0} {{ get; set; }}";

        private const string EXTENSIONS = @"public static class ETLExtensions
{
    public static string ReadString(this TextReader reader, int length)
    {
        var buffer = new char[length];
        reader.Read(buffer, 0, length);
        return new string(buffer);
    }
}
";

        private static string GenerateRecord(InputSpec spec)
        {
            var reads = string.Join(Environment.NewLine, GenerateReads(spec));
            var fields = string.Join(Environment.NewLine, GenerateFields(spec));
            return string.Format(RECORD_TEMPLATE, reads, fields);
        }

        private static IEnumerable<string> GenerateReads(InputSpec spec)
        {
            return spec.Fields.Select(field => string.Format(READ_TEMPLATE, field.Name, field.Size));
        }

        private static IEnumerable<string> GenerateFields(InputSpec spec)
        {
            return spec.Fields.Select(field => string.Format(FIELD_TEMPLATE, field.Name));
        }

        private static string GenerateExtensions()
        {
            return EXTENSIONS;
        }
    }
}