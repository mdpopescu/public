using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Renfield.Inventory.Tests
{
  public class FakeDbSet<T, TKey> : IDbSet<T>
    where T : class
  {
    public Expression Expression { get; private set; }
    public Type ElementType { get; private set; }
    public IQueryProvider Provider { get; private set; }
    public ObservableCollection<T> Local { get; private set; }

    public FakeDbSet(IEnumerable<T> list, Func<T> creator, Func<T, TKey> keySelector)
    {
      this.list = list.ToList();
      queryable = this.list.AsQueryable();
      this.creator = creator;
      this.keySelector = keySelector;

      Expression = queryable.Expression;
      ElementType = queryable.ElementType;
      Provider = queryable.Provider;
      Local = new ObservableCollection<T>(this.list);
    }

    public IEnumerator<T> GetEnumerator()
    {
      return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public T Find(params object[] keyValues)
    {
      return list
        .FindByKey(keySelector, keyValues[0])
        .FirstOrDefault();
    }

    public T Add(T entity)
    {
      list.Add(entity);
      return entity;
    }

    public T Remove(T entity)
    {
      list.Remove(entity);
      return entity;
    }

    public T Attach(T entity)
    {
      list.Add(entity);
      return entity;
    }

    public T Create()
    {
      return creator.Invoke();
    }

    public TDerivedEntity Create<TDerivedEntity>()
      where TDerivedEntity : class, T
    {
      throw new NotImplementedException();
    }

    //

    private readonly List<T> list;
    private readonly IQueryable queryable;
    private readonly Func<T> creator;
    private readonly Func<T, TKey> keySelector;
  }
}