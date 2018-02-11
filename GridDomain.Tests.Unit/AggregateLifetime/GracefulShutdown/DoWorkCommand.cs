using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.AggregateLifetime.GracefulShutdown {
    public class DoWorkCommand : Command<ShutdownTestAggregate>
    {
        public string Parameter { get; }
        public TimeSpan? Duration { get; }

        public DoWorkCommand(string aggregateId, string parameter, TimeSpan? duration) : base(aggregateId)
        {
            Parameter = parameter;
            Duration = duration;
        }
    }
}