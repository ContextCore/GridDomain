using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure
{
    public class CancelFutureEventCommand : Command
    {
        public CancelFutureEventCommand(string aggregateId, string value) : base(aggregateId)
        {
            Value = value;
        }

        public string Value { get; }
    }
}