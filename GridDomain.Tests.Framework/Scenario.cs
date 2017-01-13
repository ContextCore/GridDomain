using CommonDomain;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Framework
{
    public class Scenario
    {
        public static AggregateScenario<TAggregate, TCommandsHandler> New<TAggregate, TCommandsHandler>(TAggregate agr = null, TCommandsHandler handler = null)
            where TAggregate : class, IAggregate
            where TCommandsHandler : class, IAggregateCommandsHandler<TAggregate>
        {
            return new AggregateScenario<TAggregate, TCommandsHandler>(agr, handler);
        }
    }
}