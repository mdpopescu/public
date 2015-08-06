using System;
using System.Collections.Generic;
using Acta.Library.Models;

namespace Acta.Library.Contracts
{
  public interface IActaStorage
  {
    void Append(ActaTuple tuple);
    IEnumerable<ActaTuple> Get();
    IEnumerable<ActaTuple> GetById(Guid id);
  }
}