using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Web.Mvc;

namespace Renfield.Inventory
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
    ///   Gets the inner tag list.
    /// </summary>
    /// <value> The inner tag list. </value>
    public IEnumerable<TagBuilder> InnerTags
    {
      get { return new ReadOnlyCollection<TagBuilder>(innerTags); }
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
      var sb = new StringBuilder();
      foreach (var tag in innerTags)
        sb.Append(tag + Environment.NewLine);

      InnerHtml = sb.ToString();

      return base.ToString();
    }

    //

    private readonly IList<TagBuilder> innerTags = new List<TagBuilder>();
  }
}