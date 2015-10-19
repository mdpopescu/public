using System.Collections.Generic;

namespace Giles.Library.Models
{
  public class Entry
  {
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public IEnumerable<string> Labels { get; private set; }

    public Entry(string subject, string body, IEnumerable<string> labels)
    {
      Subject = subject;
      Body = body;
      Labels = labels;
    }
  }
}