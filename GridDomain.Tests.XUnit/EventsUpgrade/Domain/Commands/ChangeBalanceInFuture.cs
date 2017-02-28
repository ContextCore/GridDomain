using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.EventsUpgrade.Domain.Commands
{
    public class ChangeBalanceInFuture : Command
    {
        public ChangeBalanceInFuture(int parameter, Guid aggregateId, DateTime raiseTime, bool useLegacyEvent)
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