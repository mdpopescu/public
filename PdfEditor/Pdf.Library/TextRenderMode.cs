namespace Pdf.Library
{
  // http://api.itextpdf.com/itext/com/itextpdf/text/pdf/parser/TextRenderInfo.html
  public enum TextRenderMode
  {
    FillText = 0,
    StrokeText = 1,
    FillThenStrokeText = 2,
    Invisible = 3,
    FillTextAndAddToPathForClipping = 4,
    StrokeTextAndAddToPathForClipping = 5,
    FillThenStrokeTextAndAddToPathForClipping = 6,
    AddTextToPaddForClipping = 7
  }
}