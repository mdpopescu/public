namespace ExpressionCompiler.Models.Boosters
{
    public class ClosedPar : PriorityBooster
    {
        public ClosedPar()
            : base(")")
        {
            Boost = -Constants.DEPTH_BOOST;
        }

        public override int Boost { get; }
    }
}