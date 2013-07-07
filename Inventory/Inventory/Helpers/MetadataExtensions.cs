using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace Renfield.Inventory.Helpers
{
  public static class MetadataExtensions
  {
    public static string DataBinding<TModel, U, V>(this HtmlHelper<TModel> helper, Expression<Func<TModel, IEnumerable<U>>> listExpr,
      Expression<Func<U, V>> memberExpr)
    {
      var meta1 = ModelMetadata.FromLambdaExpression(listExpr, helper.ViewData);
      var listName = meta1.PropertyName;

      var itemName = GetProperty(memberExpr).Name;

      return string.Format("value: {1}, attr: {{ id : '{0}_' + $index() + '__{1}', name: '{0}[' + $index() + '].{1}' }}",
        listName, itemName);
    }

    public static MvcHtmlString KnockoutTableForModel<TModel>(this HtmlHelper<TModel> helper, string collectionName, object htmlAttributes)
    {
      var meta = helper.ViewData.ModelMetadata;
      var relevantProperties = meta
        .Properties
        .Where(it => it.ShowForDisplay && (it.ModelType.IsValueType || Type.GetTypeCode(it.ModelType) == TypeCode.String))
        .ToList();

      var table = new MultiLevelTagBuilder("table");
      table.AddAttributes(htmlAttributes);

      var thead = relevantProperties.CreateTableHeader();
      table.Add(thead);

      var tbody = relevantProperties.CreateTableBody(collectionName);
      table.Add(tbody);

      return new MvcHtmlString(table.ToString());
    }

    //

    private static PropertyInfo GetProperty<T, U>(this Expression<Func<T, U>> expression)
    {
      MemberExpression memberExpression = null;

      switch (expression.Body.NodeType)
      {
        case ExpressionType.Convert:
          memberExpression = ((UnaryExpression) expression.Body).Operand as MemberExpression;
          break;

        case ExpressionType.MemberAccess:
          memberExpression = expression.Body as MemberExpression;
          break;
      }

      if (memberExpression == null)
        throw new ArgumentException("Not a member access", "expression");

      return memberExpression.Member as PropertyInfo;
    }

    private static void AddAttributes(this TagBuilder tag, object htmlAttributes)
    {
      var properties = htmlAttributes.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
      foreach (var property in properties)
        tag.MergeAttribute(property.Name, property.GetValue(htmlAttributes, null).ToString());
    }

    private static TagBuilder CreateTableHeader(this IEnumerable<ModelMetadata> properties)
    {
      var thead = new MultiLevelTagBuilder("thead");

      var tr = new MultiLevelTagBuilder("tr");

      var columns = properties.Select(it => it.GetColumn());
      foreach (var tag in columns)
        tr.Add(tag);

      thead.Add(tr);
      return thead;
    }

    private static TagBuilder GetColumn(this ModelMetadata property)
    {
      var column = new TagBuilder("th");
      column.SetInnerText(property.DisplayName);

      return column;
    }

    private static TagBuilder CreateTableBody(this IEnumerable<ModelMetadata> properties, string collectionName)
    {
      var tbody = new MultiLevelTagBuilder("tbody");
      tbody.MergeAttribute("data-bind", "foreach: " + collectionName);

      var tr = new MultiLevelTagBuilder("tr");

      var fields = properties.Select(it => it.GetField());
      foreach (var tag in fields)
        tr.Add(tag);

      tbody.Add(tr);
      return tbody;
    }

    private static TagBuilder GetField(this ModelMetadata property)
    {
      var field = new TagBuilder("td");
      field.MergeAttribute("data-bind", "text: " + property.PropertyName);
      if (IsNumeric(property))
        field.MergeAttribute("class", "number");

      return field;
    }

    private static bool IsNumeric(ModelMetadata property)
    {
      return Type.GetTypeCode(property.ModelType) == TypeCode.Decimal ||
             property.AdditionalValues.GetValue("numeric") + "" == "numeric";
    }
  }
}