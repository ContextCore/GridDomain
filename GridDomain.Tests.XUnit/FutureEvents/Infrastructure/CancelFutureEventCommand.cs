using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.FutureEvents.Infrastructure
{
    public class CancelFutureEventCommand : Command
    {
        public CancelFutureEventCommand(Guid aggregateId, string value):base(aggregateId)
        {
            Value = value;
        }
        public string Value { get; }
    }
}