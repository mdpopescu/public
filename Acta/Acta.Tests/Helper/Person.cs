using System;
using Acta.Library.Attributes;

namespace Acta.Tests.Helper
{
  public class Person
  {
    // convention: the key is
    // - the first property marked with the [Key] attribute
    // - the Id property
    // - the {type}Id property

    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }
    public DateTime DOB { get; set; }
  }
}