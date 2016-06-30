using System;
using System.Collections.Generic;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.DependencyInjection;

namespace GridDomain.Tests.SyncProjection.SampleDomain
{
    public class TestAggregatesCommandHandler : AggregateCommandsHandler<TestAggregate>,
                                                        IAggregateCommandsHandlerDesriptor

    {
        //TODO: refactor to separate class
        public static readonly IAggregateCommandsHandlerDesriptor Descriptor = new TestAggregatesCommandHandler();
        public TestAggregatesCommandHandler() : base(null)
        {
            Map<ChangeAggregateCommand>(c => c.AggregateId,
                                       (c, a) => a.ChangeState(c.Parameter));

            Map<ChangeAggregateCommand>(c => c.AggregateId,
                                        c => new TestAggregate(c.AggregateId, c.Parameter.ToString()));
        }

        public Type AggregateType => typeof(TestAggregate);

        public IReadOnlyCollection<AggregateLookupInfo> RegisteredCommands => GetRegisteredCommands();
    }
}