using System;
using GridDomain.EventSourcing;
using NUnit.Framework;

namespace GridDomain.Tests
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void TryMe()
        {
            DomainEvent e = null;
            Console.WriteLine((e?.SagaId == Guid.Empty).ToString());
        }
    }
}