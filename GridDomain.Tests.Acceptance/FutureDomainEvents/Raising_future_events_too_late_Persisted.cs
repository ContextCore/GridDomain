using System;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.FutureEvents;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.FutureDomainEvents
{
    [TestFixture]
    public class Raising_future_events_too_late_Persisted : Raising_future_events_too_late
    {
        protected override bool CreateNodeOnEachTest { get; } = true;
        public Raising_future_events_too_late_Persisted(): base(false)
        {
        }
    }
}