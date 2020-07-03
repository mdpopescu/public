namespace DecoratorGen.Library.Models
{
    public class MethodMember : Member
    {
        public string TypeName { get; set; }
        public string Name { get; set; }
        public Argument[] Arguments { get; set; }
    }
}