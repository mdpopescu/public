using System;

namespace Renfield.Inventory.Data
{
  public interface TransactionBlock: IDisposable
  {
    void Commit();
    void Abort();
  }
}