using System;
using System.Web.Mvc;

namespace Renfield.Inventory.Helpers
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public sealed class NumericAttribute : Attribute, IMetadataAware
  {
    public void OnMetadataCreated(ModelMetadata metadata)
    {
      metadata.AdditionalValues["numeric"] = "numeric";
    }
  }
}