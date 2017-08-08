using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain
{
    public class HomeAggregateHandler : AggregateCommandsHandler<HomeAggregate>
    {
        public HomeAggregateHandler()
        {
            Map<GoSleepCommand>((c, a) => a.Sleep(c.SofaId));
        }
    }
}