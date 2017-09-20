using System;

namespace WindowsFormsApp1.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DeclareInputAttribute : Attribute
    {
        public string Category { get; }
        public string InputLabel { get; }
        public string OutputLabel { get; }

        public DeclareInputAttribute(string category)
        {
            Category = category;
        }

        public DeclareInputAttribute(string category, string inputLabel)
        {
            Category = category;
            InputLabel = inputLabel;
        }

        public DeclareInputAttribute(string category, string inputLabel, string outputLabel)
        {
            Category = category;
            InputLabel = inputLabel;
            OutputLabel = outputLabel;
        }
    }
}