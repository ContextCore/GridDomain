using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SampleDomain.ProjectionBuilders;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.CommandsExecution.ExpectedMessages
{
    [TestFixture]
    public class SyncExecute_until_projection_group_fault : When_SyncExecute_until_projection_fault
    {

        public SyncExecute_until_projection_group_fault() : base(true)
        {

        }

        public SyncExecute_until_projection_group_fault(bool inMemory = true) : base(inMemory)
        {

        }

        protected override IMessageRouteMap CreateMap()
        {
            var faultyHandlerMap = new CustomRouteMap(r => r.RegisterProjectionGroup(new TestGroup(new UnityContainer())),
                                                      r => r.RegisterAggregate(SampleAggregatesCommandHandler.Descriptor));

            return new CompositeRouteMap(faultyHandlerMap);
        }

        public class TestGroup : ProjectionGroup
        {
            public TestGroup(IUnityContainer locator) : base(locator)
            {
                this.Add<SampleAggregateChangedEvent, OddFaultyMessageHandler>(e => e.SourceId);
                this.Add<SampleAggregateChangedEvent, EvenFaultyMessageHandler>(e => e.SourceId);
            }
        }
    }
}