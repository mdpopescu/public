using System;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace Pdf.Library
{
  public class PdfProcessor
  {
    public string ConvertToHtml(PdfReader reader, int pageNumber)
    {
      var strategy = new TextBlockStrategy();
      PdfTextExtractor.GetTextFromPage(reader, pageNumber, strategy);

      strategy.FlipYCoordinate();
      var text = string.Join(Environment.NewLine, strategy.GetBlocks().Select(block => block.ToString()));

      return string.Format("<div style='position: relative;'>{0}</div>", text);
    }
  }
}