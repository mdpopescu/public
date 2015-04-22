using CQRS.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CQRS.Tests
{
  [TestClass]
  public class EventBusTests
  {
    [TestMethod]
    public void ObjectsCanSubscribeToEvents()
    {
      var obj = new MyClass1();

      EventBus.Send("E1", 5);

      Assert.AreEqual(5, obj.X);
    }

    //

    private class MyClass1
    {
      public int X { get; private set; }

      public MyClass1()
      {
        EventBus.Subscribe(this, "E1");
      }

      public void E1(int x)
      {
        X = x;
      }
    }
  }
}