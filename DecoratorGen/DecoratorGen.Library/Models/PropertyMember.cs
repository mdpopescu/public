namespace DecoratorGen.Library.Models
{
    public class PropertyMember : Member
    {
        public virtual string TypeName { get; set; }
        public virtual string Name { get; set; }
        public virtual bool HasGetter { get; set; }
        public virtual bool HasSetter { get; set; }
    }
}