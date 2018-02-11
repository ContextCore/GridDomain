using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain.Commands
{
    public class ChangeBalanceInFuture : Command<BalanceAggregatesCommandHandler>
    {
        public ChangeBalanceInFuture(int parameter, string aggregateId, DateTime raiseTime, bool useLegacyEvent)
            : base(aggregateId)
        {
            Parameter = parameter;
            RaiseTime = raiseTime;
            UseLegacyEvent = useLegacyEvent;
        }

        public DateTime RaiseTime { get; }
        public bool UseLegacyEvent { get; }
        public int Parameter { get; }
    }
}