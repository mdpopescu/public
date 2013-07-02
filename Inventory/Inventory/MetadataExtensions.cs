using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace Renfield.Inventory
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

    public static string GetPropertyName<T>(this Expression<Func<T>> property)
    {
      var propertyInfo = (property.Body as MemberExpression).Member as PropertyInfo;
      if (propertyInfo == null)
        throw new ArgumentException("The lambda expression 'property' should point to a valid Property");

      return propertyInfo.Name;
    }

    public static PropertyInfo GetProperty<T, U>(this Expression<Func<T, U>> expression)
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
  }
}