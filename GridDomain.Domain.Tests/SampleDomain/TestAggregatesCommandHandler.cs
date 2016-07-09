using System;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.SampleDomain
{
    public class TestAggregatesCommandHandler : AggregateCommandsHandler<SampleAggregate>,
                                                        IAggregateCommandsHandlerDesriptor

    {
        //TODO: refactor to separate class
        public static readonly IAggregateCommandsHandlerDesriptor Descriptor = new TestAggregatesCommandHandler();
        public TestAggregatesCommandHandler() : base(null)
        {
            Map<ChangeAggregateCommand>(c => c.AggregateId,
                                       (c, a) => a.ChangeState(c.Parameter));

            Map<CreateAggregateCommand>(c => c.AggregateId,
                                        c => new SampleAggregate(c.AggregateId, c.Parameter.ToString()));

            Map<LongOperationCommand>(c => c.AggregateId,
                                     (c, a) => a.LongExecute(c.Parameter));
        }

        public Type AggregateType => typeof(SampleAggregate);
    }
}