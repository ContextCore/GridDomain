using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure
{
    public class ValueChangedSuccessfullyEvent : DomainEvent
    {
        public string Value { get; }
        public int RetriesToSucceed { get; }

        public ValueChangedSuccessfullyEvent(string value,
                               int retriesToSucceed,
                                             string sourceId,
                               DateTime? createdTime = default(DateTime?),
                                             string processId = null) : base(sourceId, processId, null, createdTime)
        {
            Value = value;
            RetriesToSucceed = retriesToSucceed;
        }

    }
}