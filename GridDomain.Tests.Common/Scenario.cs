using GridDomain.EventSourcing;

namespace GridDomain.Tests.Common
{
    public class Scenario
    {
        public static AggregateScenario<TAggregate, TCommandsHandler> New<TAggregate, TCommandsHandler>(
            TAggregate agr = null,
            TCommandsHandler handler = null) where TAggregate : Aggregate
                                             where TCommandsHandler : class, IAggregateCommandsHandler<TAggregate>
        {
            return new AggregateScenario<TAggregate, TCommandsHandler>(agr, handler);
        }
    }
}