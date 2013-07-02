using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Renfield.Inventory.Tests
{
  public class FakeDbSet<T> : IDbSet<T>
    where T : class, new()
  {
    public Expression Expression { get; private set; }
    public Type ElementType { get; private set; }
    public IQueryProvider Provider { get; private set; }
    public ObservableCollection<T> Local { get; private set; }

    public FakeDbSet(List<T> list)
    {
      this.list = list;
      queryable = this.list.AsQueryable();

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
      throw new NotImplementedException();
    }

    public T Add(T entity)
    {
      list.Add(entity);

      FixEntityId(entity);
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

      FixEntityId(entity);
      return entity;
    }

    public T Create()
    {
      return new T();
    }

    public TDerivedEntity Create<TDerivedEntity>()
      where TDerivedEntity : class, T
    {
      throw new NotImplementedException();
    }

    //

    private readonly List<T> list;
    private readonly IQueryable queryable;

    private void FixEntityId(T entity)
    {
      // all my entities have "int Id" as the identity key, so set that
      dynamic it = entity;
      it.Id = list.Select(item => ((dynamic) item).Id).Max() + 1;
    }
  }
}