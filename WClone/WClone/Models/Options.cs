namespace WClone.Models
{
    public class Options
    {
        public Options()
        {
            MaxDepth = int.MaxValue;
        }

        public int MaxDepth { get; set; }
    }
}