using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Events;
using GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.MessageWaiting.Commanding
{
    [TestFixture]
    public class CommandWaiterWaitsForFaultsFromProjectionGroup : CommandWaiter_waits_for_faults
    {


        protected override IMessageRouteMap CreateMap()
        {
            var faultyHandlerMap = new CustomRouteMap(r => r.RegisterProjectionGroup(new TestGroup(new UnityContainer())),
                                                      r => r.RegisterAggregate(SampleAggregatesCommandHandler.Descriptor));

            return new CompositeRouteMap(faultyHandlerMap);
        }

        public class TestGroup : ProjectionGroup
        {
            public TestGroup(IUnityContainer locator = null) : base(locator)
            {
                this.Add<SampleAggregateChangedEvent, OddFaultyMessageHandler>(e => e.SourceId);
                this.Add<SampleAggregateChangedEvent, EvenFaultyMessageHandler>(e => e.SourceId);
            }
        }
    }
}