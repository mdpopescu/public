using System.IO;
using System.Linq;
using Inventory2.InMemoryState.Library.Commands;
using Inventory2.InMemoryState.Library.Core;
using Inventory2.Library.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inventory2.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod]
        public void Scenario1()
        {
            using (var ms = new MemoryStream())
            {
                var events = new WORMStream(ms);
                var serializer = new EventSerializer();
                var framework = new Framework<InventoryState>(events, serializer);

                var id1 = framework
                    .Execute(new CreateProduct("test1"))
                    .Select(ev => ev.EntityId)
                    .First();
                framework.Execute(new BuyProduct(id1, 10m, 20m));

                var products = framework
                    .State
                    .Products
                    .ToList();
                Assert.AreEqual(1,products.Count);
                Assert.AreEqual("test1",products[0].Name);
            }
        }
    }
}