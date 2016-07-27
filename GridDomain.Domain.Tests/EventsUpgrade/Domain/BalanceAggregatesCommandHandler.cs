using System;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.EventsUpgrade.Domain.Commands;

namespace GridDomain.Tests.EventsUpgrade.Domain
{
    public class BalanceAggregatesCommandHandler: AggregateCommandsHandler<BalanceAggregate>,
                                                        IAggregateCommandsHandlerDesriptor

    {
        //TODO: refactor to separate class
        public static readonly IAggregateCommandsHandlerDesriptor Descriptor = new BalanceAggregatesCommandHandler();
        public BalanceAggregatesCommandHandler() : base(null)
        {
            Map<ChangeBalanceCommand>(c => c.AggregateId,
                                       (c, a) => a.ChangeState(c.Parameter));

            Map<CreateBalanceCommand>(c => c.AggregateId,
                                        c => new BalanceAggregate(c.AggregateId, c.Parameter));
        }

        public Type AggregateType => typeof(BalanceAggregate);
    }
}