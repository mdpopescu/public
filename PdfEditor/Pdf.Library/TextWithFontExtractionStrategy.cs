using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf.parser;

namespace Pdf.Library
{
  // based on http://stackoverflow.com/a/6884297/31793
  public class TextWithFontExtractionStrategy : ITextExtractionStrategy
  {
    public void RenderText(TextRenderInfo renderInfo)
    {
      var curFont = renderInfo.GetFont().PostscriptFontName;

      //Check if faux bold is used
      if ((renderInfo.GetTextRenderMode() == (int) TextRenderMode.FillThenStrokeText))
        curFont += "-Bold";

      //This code assumes that if the baseline changes then we're on a newline
      var curBaseline = renderInfo.GetBaseline().GetStartPoint();
      var topRight = renderInfo.GetAscentLine().GetEndPoint();
      var rect = new Rectangle(curBaseline[Vector.I1], curBaseline[Vector.I2], topRight[Vector.I1], topRight[Vector.I2]);
      var curFontSize = rect.Height;

      //See if something has changed, either the baseline, the font or the font size
      if ((lastBaseLine == null) || (curBaseline[Vector.I2] != lastBaseLine[Vector.I2]) || (curFontSize != lastFontSize) ||
          (curFont != lastFont))
      {
        //if we've put down at least one span tag close it
        if ((lastBaseLine != null))
          result.AppendLine("</span>");

        //If the baseline has changed then insert a line break
        if ((lastBaseLine != null) && curBaseline[Vector.I2] != lastBaseLine[Vector.I2])
          result.AppendLine("<br />");

        //Create an HTML tag with appropriate styles
        result.AppendFormat("<span style=\"font-family:{0};font-size:{1}; position: relative; top: {2}; left: {3};\">",
          curFont, curFontSize, 850 - rect.Top, rect.Left);
      }

      //Append the current text
      result.Append(renderInfo.GetText());

      //Set currently used properties
      lastBaseLine = curBaseline;
      lastFontSize = curFontSize;
      lastFont = curFont;
    }

    public string GetResultantText()
    {
      //If we wrote anything then we'll always have a missing closing tag so close it here
      if (result.Length > 0)
        result.Append("</span>");

      return result.ToString();
    }

    //Not needed
    public void BeginTextBlock()
    {
    }

    public void EndTextBlock()
    {
    }

    public void RenderImage(ImageRenderInfo renderInfo)
    {
    }

    //

    //HTML buffer
    private readonly StringBuilder result = new StringBuilder();

    //Store last used properties
    private Vector lastBaseLine;
    private string lastFont;
    private float lastFontSize;
  }
}