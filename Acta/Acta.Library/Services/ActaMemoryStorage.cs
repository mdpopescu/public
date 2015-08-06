using System;
using System.Collections.Generic;
using System.Linq;
using Acta.Library.Contracts;
using Acta.Library.Models;

namespace Acta.Library.Services
{
  public class ActaMemoryStorage : IActaStorage
  {
    public ActaMemoryStorage()
    {
      list = new List<ActaTuple>();
    }

    public void Append(ActaTuple tuple)
    {
      list.Add(tuple);
    }

    public IEnumerable<ActaTuple> Get()
    {
      return list.AsEnumerable();
    }

    public IEnumerable<ActaTuple> GetById(Guid id)
    {
      return list.Where(it => it.Id == id);
    }

    //

    private readonly List<ActaTuple> list;
  }
}