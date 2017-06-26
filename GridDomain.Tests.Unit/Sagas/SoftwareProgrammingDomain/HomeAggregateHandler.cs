using System;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Commands;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain
{
    public class HomeAggregateHandler : AggregateCommandsHandler<HomeAggregate>,
                                        IAggregateCommandsHandlerDescriptor
    {
        public static readonly IAggregateCommandsHandlerDescriptor Descriptor = new HomeAggregateHandler();

        public HomeAggregateHandler()
        {
            Map<GoSleepCommand>((c, a) => a.Sleep(c.SofaId));
        }

        public Type AggregateType => typeof(HomeAggregate);
    }
}