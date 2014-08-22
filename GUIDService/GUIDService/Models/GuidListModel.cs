using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Renfield.GUIDService.Models
{
  public class GuidListModel
  {
    [Display(Name = "How many GUIDs to generate?")]
    public int Count { get; set; }

    [Display(Name = "Total run time:")]
    [DataType(DataType.Duration)]
    public TimeSpan Duration { get; set; }

    [Display(Name = "First 10 GUIDs:")]
    public IEnumerable<string> Values { get; set; }

    public GuidListModel()
    {
      Values = Enumerable.Empty<string>();
    }
  }
}