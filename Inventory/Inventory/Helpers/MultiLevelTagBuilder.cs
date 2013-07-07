using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace Renfield.Inventory.Helpers
{
  // from http://codereview.stackexchange.com/a/5528/27018
  /// <summary>
  ///   Simple class that inherits TagBuilder and allow to save sub tags.
  /// </summary>
  public class MultiLevelTagBuilder : TagBuilder
  {
    /// <summary>
    ///   Initializes a new instance of the <see cref="MultiLevelTagBuilder" /> class.
    /// </summary>
    /// <param name="tagName"> The name of the tag. </param>
    public MultiLevelTagBuilder(string tagName)
      : base(tagName)
    {
    }

    /// <summary>
    ///   Adds the specified tag to the inner tag list.
    /// </summary>
    /// <param name="tag"> The tag to add. </param>
    public void Add(TagBuilder tag)
    {
      if (tag == null)
        throw new ArgumentNullException("tag");

      innerTags.Add(tag);
    }

    /// <summary>
    ///   Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns> A <see cref="System.String" /> that represents this instance. </returns>
    public override string ToString()
    {
      return ToString(0);
    }

    //

    private static readonly Func<int, string> GetIndent = Memoizer.Memoize<int, string>(Indent);

    private readonly IList<TagBuilder> innerTags = new List<TagBuilder>();

    private string ToString(int level)
    {
      var sb = new StringBuilder();
      foreach (var tag in innerTags)
      {
        var multiTag = tag as MultiLevelTagBuilder;
        if (multiTag != null)
          sb.AppendLine(multiTag.ToString(level + 1));
        else
          sb.AppendLine(GetIndent(level + 1) + tag);
      }

      InnerHtml = sb.ToString();

      return Prettified(level);
    }

    private string Prettified(int level)
    {
      return InnerHtml == ""
               ? GetIndent(level + 1) + base.ToString()
               : string.Format("{0}{1}{2}{3}{0}{4}",
                 GetIndent(level),
                 ToString(TagRenderMode.StartTag),
                 Environment.NewLine,
                 InnerHtml,
                 ToString(TagRenderMode.EndTag));
    }

    private static string Indent(int level)
    {
      return new string(' ', level * 2);
    }
  }
}