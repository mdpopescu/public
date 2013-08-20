using System.Transactions;

namespace Renfield.Inventory.Data
{
  public class TransactionScopeWrapper : TransactionBlock
  {
    public TransactionScopeWrapper(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
      var options = new TransactionOptions
      {
        IsolationLevel = isolationLevel,
        Timeout = TransactionManager.DefaultTimeout,
      };

      ts = new TransactionScope(TransactionScopeOption.Required, options);
    }

    public void Commit()
    {
      ts.Complete();
    }

    public void Abort()
    {
      ts.Dispose();
    }

    public void Dispose()
    {
      Abort();
    }

    //

    private readonly TransactionScope ts;
  }
}