namespace WindowsFormsApp1.Models
{
    public class InputSelection
    {
        public string Category { get; }
        public string InputLabel { get; }
        public string OutputLabel { get; }

        public InputSelection(string category)
        {
            Category = category;
        }

        public InputSelection(string category, string inputLabel)
        {
            Category = category;
            InputLabel = inputLabel;
        }

        public InputSelection(string category, string inputLabel, string outputLabel)
        {
            Category = category;
            InputLabel = inputLabel;
            OutputLabel = outputLabel;
        }
    }
}