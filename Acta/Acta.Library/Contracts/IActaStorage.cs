using Acta.Library.Models;

namespace Acta.Library.Contracts
{
  public interface IActaStorage
  {
    void Append(ActaTuple tuple);
  }
}