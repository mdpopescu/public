namespace DecoratorGen.Library.Models
{
    public class ReadOnlyPropertyMember : PropertyMember
    {
        public override bool HasGetter
        {
            get => true;
            set { }
        }

        public override bool HasSetter
        {
            get => false;
            set { }
        }
    }
}