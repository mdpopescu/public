using System;
using System.Linq;

namespace ISBN.Library
{
  public class Converter
  {
    public string Convert(string productId)
    {
      if (productId == null || productId.Length != 12)
        throw new Exception("Invalid productId: " + productId);

      // remove the first three digits
      var result = productId.Substring(3);

      // add the control digit
      result += GetControlDigit(result);

      return result;
    }

    //

    private string GetControlDigit(string s)
    {
      var sum = s
        .Select(c => int.Parse(c.ToString()))
        .Select((digit, index) => digit * (10 - index))
        .Sum();
      var mod = sum % 11;

      var result = (11 - mod) % 11;

      return result == 10 ? "x" : result.ToString();
    }
  }
}