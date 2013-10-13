using System.Collections.Generic;
using System.Collections.ObjectModel;
using iTextSharp.text;
using iTextSharp.text.pdf.parser;

namespace Pdf.Library
{
  public class TextBlockStrategy : ITextExtractionStrategy
  {
    public const int PAGE_HEIGHT = 900;

    public TextBlockStrategy()
    {
      blocks = new List<TextBlock>();
    }

    public void FlipYCoordinate()
    {
      foreach (var block in blocks)
      {
        var rect = block.Rectangle;
        block.Rectangle = new Rectangle(rect.Left, /*PAGE_HEIGHT - */rect.Top, rect.Right, rect.Bottom);
      }
    }

    public ReadOnlyCollection<TextBlock> GetBlocks()
    {
      return blocks.AsReadOnly();
    }

    #region ITextExtractionStrategy

    public void BeginTextBlock()
    {
      //
    }

    public void RenderText(TextRenderInfo renderInfo)
    {
      var block = new TextBlock();

      block.FontName = renderInfo.GetFont().PostscriptFontName;
      // Check if faux bold is used
      if ((renderInfo.GetTextRenderMode() == (int) TextRenderMode.FillThenStrokeText))
        block.ForceBold = true;

      var curBaseline = renderInfo.GetBaseline().GetStartPoint();
      var topRight = renderInfo.GetAscentLine().GetEndPoint();
      var rect = MakeRectangle(curBaseline, topRight);
      block.FontSize = rect.Height;
      //block.FontSize = rect.Height * 1.32f;
      block.Rectangle = rect;

      block.Value = renderInfo.GetText();

      blocks.Add(block);
    }

    public void EndTextBlock()
    {
      //
    }

    public void RenderImage(ImageRenderInfo renderInfo)
    {
      //
    }

    public string GetResultantText()
    {
      return "";
    }

    #endregion

    //

    private readonly List<TextBlock> blocks;

    private static Rectangle MakeRectangle(Vector curBaseline, Vector topRight)
    {
      return new Rectangle(curBaseline[Vector.I1], curBaseline[Vector.I2], topRight[Vector.I1], topRight[Vector.I2]);
    }
  }
}