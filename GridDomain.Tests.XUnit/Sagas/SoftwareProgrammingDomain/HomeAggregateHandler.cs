using System;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Commands;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain
{
    public class HomeAggregateHandler : AggregateCommandsHandler<HomeAggregate>, IAggregateCommandsHandlerDescriptor
    {
        public HomeAggregateHandler()
        {
            Map<GoSleepCommand>((c,a) => a.Sleep(c.SofaId));
        }

        public Type AggregateType => typeof(HomeAggregate);

        public static readonly IAggregateCommandsHandlerDescriptor Descriptor = new HomeAggregateHandler();
    }
}