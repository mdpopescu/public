namespace Renfield.RecursiveCompare
{
  public class Comparison
  {
    public ComparisonResult Result { get; private set; }
    public string PropName { get; private set; }
    public object LValue { get; private set; }
    public object RValue { get; private set; }

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