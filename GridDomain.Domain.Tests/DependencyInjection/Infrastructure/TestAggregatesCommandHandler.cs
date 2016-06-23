using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.DependencyInjection
{
    public class TestAggregatesCommandHandler : AggregateCommandsHandler<TestAggregate>
    {
        public TestAggregatesCommandHandler()
        {
            Map<TestCommand>(c => c.AggregateId, 
                            (c, a) => a.Execute(c.Parameter, null));
        }
    }
}