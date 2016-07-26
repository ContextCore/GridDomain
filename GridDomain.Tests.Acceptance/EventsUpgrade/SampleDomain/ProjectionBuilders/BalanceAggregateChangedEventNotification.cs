using System;

namespace GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.ProjectionBuilders
{
    public class BalanceAggregateChangedEventNotification
    {
        public Guid AggregateId { get; set; }
    }
}