namespace TransformyClone.Library
{
  public static class StringExtensions
  {
    public static string DuplicateCurlyBraces(this string s)
    {
      return s
        .Replace("{", "{{")
        .Replace("}", "}}");
    }
  }
}