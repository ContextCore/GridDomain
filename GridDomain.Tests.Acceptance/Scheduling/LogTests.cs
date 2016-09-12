using System;
using GridDomain.Logging;
using NMoneys;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Scheduling
{
    [TestFixture]
    public class LogTests
    {
        [Test]
        public void LogTest()
        {
            TypesForScalarDestructionHolder.Add(typeof(Money));
            LogManager.SetLoggerFactory(new DefaultLoggerFactory());
            LogManager.GetLogger().Error(new InvalidOperationException("ohshitwaddap"), "MONEY TEST {@placeholder}", new { Money = new Money(123, CurrencyIsoCode.RUB) });
        }
    }
}