namespace WindowsFormsApp2.Models
{
    public class LabeledValue
    {
        public string Id { get; }
        public string Label { get; }
        public object Value { get; }

        public LabeledValue(string id, string label, object value)
        {
            Id = id;
            Label = label;
            Value = value;
        }

        public override string ToString() => $"{Id}:{Label} = [{Value ?? "null"}]";
    }
}