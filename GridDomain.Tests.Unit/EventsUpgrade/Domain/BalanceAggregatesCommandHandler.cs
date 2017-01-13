using System;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Commands;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain
{
    public class BalanceAggregatesCommandHandler: AggregateCommandsHandler<BalanceAggregate>,
                                                        IAggregateCommandsHandlerDescriptor

    {
        //TODO: refactor to separate class
        public static readonly IAggregateCommandsHandlerDescriptor Descriptor = new BalanceAggregatesCommandHandler();
        public BalanceAggregatesCommandHandler() : base()
        {
            Map<ChangeBalanceCommand>(c => c.AggregateId,
                                       (c, a) => a.ChangeState(c.Parameter));

            Map<CreateBalanceCommand>(c => c.AggregateId,
                                        c => new BalanceAggregate(c.AggregateId, c.Parameter));

            Map<ChangeBalanceInFuture>(c => c.AggregateId,
                                         (c, a) => a.ChangeStateInFuture(c.RaiseTime, c.Parameter, c.UseLegacyEvent));
            this.MapFutureEvents();
        }

        public Type AggregateType => typeof(BalanceAggregate);
    }
}