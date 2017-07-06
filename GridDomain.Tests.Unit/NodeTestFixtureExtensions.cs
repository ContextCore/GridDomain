using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using Microsoft.Practices.Unity;
using Quartz;

namespace GridDomain.Tests.Unit
{
    public static class NodeTestFixtureExtensions
    {
        public static void ClearSheduledJobs(this NodeTestFixture fixture)
        {
            fixture.OnNodeStartedEvent += (sender, args) => fixture.Node.Container.Resolve<IScheduler>().Clear();
        }

        public static NodeTestFixture UseAdaper<TFrom,TTo>(this NodeTestFixture fixture, ObjectAdapter<TFrom,TTo> adapter)
        {
            fixture.OnNodeCreatedEvent += (sender, node) => node.EventsAdaptersCatalog.Register(adapter);
            return fixture;
        }

        public static NodeTestFixture UseAdaper<TFrom,TTo>(this NodeTestFixture fixture, DomainEventAdapter<TFrom,TTo> adapter) where TFrom : DomainEvent
                                                                                                                                where TTo : DomainEvent
        {
            fixture.OnNodeCreatedEvent += (sender, node) => node.EventsAdaptersCatalog.Register(adapter);
            return fixture;
        }

    }
}