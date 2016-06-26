using System;
using System.Collections.Generic;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.DependencyInjection;

namespace GridDomain.Tests.FutureEvents.Infrastructure
{
    public class TestAggregatesCommandHandler : AggregateCommandsHandler<TestAggregate>,
                                                IAggregateCommandsHandlerDesriptor

    {
        public static readonly IAggregateCommandsHandlerDesriptor Descriptor = new TestAggregatesCommandHandler(null);
        public TestAggregatesCommandHandler(IServiceLocator serviceLocator) : base(serviceLocator)
        {
            Map<TestCommand>(c => c.AggregateId,
                            (c, a) => a.RaiseFutureEvent(c.RaiseTime));
        }

        public Type AggregateType => typeof(TestAggregate);
    }
}