using System;
using AdNetwork.Library.Contracts;
using AdNetwork.Library.Models;
using AutoBogus;
using Moq;

namespace AdNetwork.Tests
{
    public static class ObjectMother
    {
        public static ServiceBuildInfo CreateService() =>
            new ServiceBuildInfo();

        public static ServiceBuildInfo WithDelay(this ServiceBuildInfo info, TimeSpan delay)
        {
            info.Delay = delay;
            return info;
        }

        public static ServiceBuildInfo WithAmount(this ServiceBuildInfo info, decimal amount)
        {
            info.Amount = amount;
            return info;
        }

        public static Mock<IAdService> Build(this ServiceBuildInfo info, out string html)
        {
            html = AutoFaker.Generate<string>();
            var service = new Mock<IAdService>();
            service
                .Setup(it => it.GetOffer(It.IsAny<Criteria>()))
                .ReturnsAsync(new AdResponse { Amount = info.Amount, Html = html }, info.Delay);
            return service;
        }

        //

        public class ServiceBuildInfo
        {
            public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(1);
            public decimal Amount { get; set; } = 0.0001m;
        }
    }
}