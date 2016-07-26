using System;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.Commands;

namespace GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain
{
    public class SampleAggregatesCommandHandler: AggregateCommandsHandler<BalanceAggregate>,
                                                        IAggregateCommandsHandlerDesriptor

    {
        //TODO: refactor to separate class
        public static readonly IAggregateCommandsHandlerDesriptor Descriptor = new SampleAggregatesCommandHandler();
        public SampleAggregatesCommandHandler() : base(null)
        {
            Map<ChangeAggregateCommand>(c => c.AggregateId,
                                       (c, a) => a.ChangeState(c.Parameter));

            Map<CreateAggregateCommand>(c => c.AggregateId,
                                        c => new BalanceAggregate(c.AggregateId, c.Parameter));
        }

        public Type AggregateType => typeof(BalanceAggregate);
    }
}