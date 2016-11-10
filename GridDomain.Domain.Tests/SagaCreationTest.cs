using System;
using System.Threading;
using GridDomain.Tests.FutureEvents;
using NUnit.Framework;

namespace GridDomain.Tests
{
    [TestFixture]
    public class SagaCreationTest : FutureEventsTest
    {
        [Test]
        public void Do_schedule()
        {
           Thread.Sleep(Timeout); 
        }

        public SagaCreationTest() : base(false)
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromMinutes(200);
    }
}