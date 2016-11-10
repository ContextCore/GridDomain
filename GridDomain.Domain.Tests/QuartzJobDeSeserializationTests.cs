using System;
using System.Threading;
using GridDomain.Tests.FutureEvents;
using NUnit.Framework;

namespace GridDomain.Tests
{
    [TestFixture]
    public class QuartzJobDeSeserializationTests : FutureEventsTest
    {
        [Test]
        public void QuartzJob_should_de_deserialized_from_old_wire_format()
        {

            Thread.Sleep(10000000);
        }

        public QuartzJobDeSeserializationTests() : base(false)
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromMinutes(1);
    }
}