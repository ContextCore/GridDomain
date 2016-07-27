using System;

namespace GridDomain.Tests.EventsUpgrade.Domain.ProjectionBuilders
{
    public class BalanceAggregateChangedEventNotification
    {
        public Guid AggregateId { get; set; }
    }
}