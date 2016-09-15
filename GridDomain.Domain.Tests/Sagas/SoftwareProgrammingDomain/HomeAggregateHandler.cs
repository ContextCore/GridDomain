using System;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands
{
    public class HomeAggregateHandler : AggregateCommandsHandler<HomeAggregate>, IAggregateCommandsHandlerDesriptor
    {
        public HomeAggregateHandler()
        {
            Map<GoSleepCommand>(c => c.Id, (c,a) => a.Sleep(c.SofaId));
        }

        public Type AggregateType => typeof(HomeAggregate);

        public static readonly IAggregateCommandsHandlerDesriptor Descriptor = new HomeAggregateHandler();
    }
}