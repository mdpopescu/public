using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AdNetwork.Library;
using AdNetwork.Library.Contracts;
using AdNetwork.Library.Models;
using AdNetwork.Library.Services;
using AutoBogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AdNetwork.Tests.Services
{
    [TestClass]
    public class AppTests
    {
        private App sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new App();
        }

        [TestClass]
        public class Get : AppTests
        {
            [TestMethod]
            public async Task CallsAllRegisteredServices()
            {
                var services = new[]
                {
                    ObjectMother.CreateService().Build(out _),
                    ObjectMother.CreateService().Build(out _),
                    ObjectMother.CreateService().Build(out _),
                };
                RegisterAll(services);
                var criteria = AutoFaker.Generate<Criteria>();

                await sut.Get(criteria).ConfigureAwait(false);

                foreach (var service in services)
                    service.Verify(it => it.GetOffer(criteria));
            }

            [TestMethod]
            public async Task CallsTheServicesInParallel()
            {
                var services = new[]
                {
                    ObjectMother.CreateService().WithDelay(100).Build(out _),
                    ObjectMother.CreateService().WithDelay(110).Build(out _),
                    ObjectMother.CreateService().WithDelay(120).Build(out _),
                };
                RegisterAll(services);

                var runTime = await Benchmark(() => sut.Get(new Criteria())).ConfigureAwait(false);

                // the total time should be smaller than the sum of the time spans
                Assert.IsTrue(runTime <= 200);
            }

            [TestMethod]
            public async Task ReturnsTheResultOfTheCheapestService()
            {
                var services = new[]
                {
                    ObjectMother.CreateService().WithAmount(0.0015m).Build(out _),
                    ObjectMother.CreateService().WithAmount(0.0010m).Build(out var html),
                    ObjectMother.CreateService().WithAmount(0.0025m).Build(out _),
                };
                RegisterAll(services);

                var result = await sut.Get(new Criteria()).ConfigureAwait(false);

                Assert.AreEqual(html, result);
            }

            [TestMethod]
            public async Task ReturnsAPredefinedFragmentIfNoServiceExists()
            {
                var result = await sut.Get(new Criteria()).ConfigureAwait(false);

                Assert.AreEqual(Constants.DEFAULT_HTML, result);
            }

            [TestMethod]
            public async Task IgnoresServicesThatAreTooSlowEvenIfCheaper()
            {
                var services = new[]
                {
                    ObjectMother.CreateService().WithAmount(0.0025m).WithDelay(100).Build(out _),
                    ObjectMother.CreateService().WithAmount(0.0010m).WithDelay(500).Build(out _),
                    ObjectMother.CreateService().WithAmount(0.0015m).WithDelay(120).Build(out var html),
                };
                RegisterAll(services);

                var result = await sut.Get(new Criteria()).ConfigureAwait(false);

                Assert.AreEqual(html, result);
            }
        }

        //

        private void RegisterAll(IEnumerable<Mock<IAdService>> services)
        {
            foreach (var service in services)
                sut.Register(service.Object);
        }

        private static async Task<long> Benchmark(Func<Task> task)
        {
            var sw = new Stopwatch();
            sw.Start();
            await task.Invoke().ConfigureAwait(false);
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }
}