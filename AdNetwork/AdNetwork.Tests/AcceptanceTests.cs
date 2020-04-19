using System.Threading.Tasks;
using AdNetwork.Library.Contracts;
using AdNetwork.Library.Models;
using AdNetwork.Library.Services;
using AutoBogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AdNetwork.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod]
        public async Task Scenario1()
        {
            // the incoming request will contain some criteria
            // these criteria will be passed on to the registered services,
            // which will return an amount they are willing to pay and a HTML fragment that will display the ad

            var app = new App();

            var criteria = new Criteria();
            var svc1 = Register(app, criteria, 0.0005m, out _);
            var svc2 = Register(app, criteria, 0.0003m, out var html);
            var svc3 = Register(app, criteria, 0.0007m, out _);

            var adServed = await app.Get(criteria).ConfigureAwait(false);

            // svc2 should have won
            Assert.AreEqual(html, adServed);

            // verify that all services have been contacted
            svc1.Verify();
            svc2.Verify();
            svc3.Verify();
        }

        //

        private static Mock<IAdService> Register(App app, Criteria criteria, decimal amount, out string html)
        {
            var service = new Mock<IAdService>();
            html = AutoFaker.Generate<string>();
            var response = new AdResponse { Amount = amount, Html = html };
            service.Setup(it => it.GetOffer(criteria)).ReturnsAsync(response).Verifiable();
            app.Register(service.Object);
            return service;
        }
    }
}