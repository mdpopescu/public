namespace ExpressionCompiler.Models.Boosters
{
    public class OpenPar : PriorityBooster
    {
        public OpenPar()
            : base("(")
        {
            Boost = Constants.DEPTH_BOOST;
        }

        public override int Boost { get; }
    }
}