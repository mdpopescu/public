using iTextSharp.text;

namespace Pdf.Library
{
  public class TextBlock
  {
    public string FontName
    {
      get { return fontName; }
      set { SetFontName(value); }
    }

    public float FontSize { get; set; }
    public bool ForceBold { get; set; }
    public Rectangle Rectangle { get; set; }
    public string Value { get; set; }

    public override string ToString()
    {
      return
        string.Format(
          "<span style='font-family: {0}; font-size: {1}; {2}position: absolute; top: {3}; left: {4};'>{5}</span>",
          FontName, FontSize, ForceBold ? "font-weight:bold; " : "", (int) Rectangle.Top, (int) Rectangle.Left, Value);
    }

    //

    private string fontName;

    private void SetFontName(string name)
    {
      if (name.Contains("Arial"))
        fontName = "Arial";
      else
        fontName = name;

      if (name.Contains("-Bold"))
        ForceBold = true;
    }
  }
}