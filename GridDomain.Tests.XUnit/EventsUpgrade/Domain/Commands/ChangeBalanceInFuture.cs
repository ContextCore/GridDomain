using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.EventsUpgrade.Domain.Commands
{
    public class ChangeBalanceInFuture : Command
    {
        public ChangeBalanceInFuture(int parameter, Guid aggregateId, DateTime raiseTime, bool useLegacyEvent)
        {
            Parameter = parameter;
            AggregateId = aggregateId;
            RaiseTime = raiseTime;
            UseLegacyEvent = useLegacyEvent;
        }

        public Guid AggregateId { get; }
        public DateTime RaiseTime { get; }
        public bool UseLegacyEvent { get; }
        public int Parameter { get; }
    }
}