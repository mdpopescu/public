namespace Renfield.RecursiveCompare
{
    public class Comparison
    {
        public ComparisonResult Result { get; }
        public string PropName { get; }
        public object LValue { get; }
        public object RValue { get; }

        public Comparison(string propName, object lValue, object rValue)
        {
            Result = GetComparisonResult(lValue, rValue);
            PropName = propName;
            LValue = lValue;
            RValue = rValue;
        }

        //

        private static ComparisonResult GetComparisonResult(object obj1, object obj2)
        {
            return obj1 == obj2
                ? ComparisonResult.Equal
                : obj1 == null || obj2 == null
                    ? ComparisonResult.NotEqual
                    : obj1.Equals(obj2) ? ComparisonResult.Equal : ComparisonResult.NotEqual;
        }
    }
}