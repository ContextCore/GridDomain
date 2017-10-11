using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Aggregate_actor_lifetime
{
    public class ProcessHub_children_lifetime_tests : PersistentHubChildrenLifetimeTest
    {
        public ProcessHub_children_lifetime_tests(ITestOutputHelper output)
            : base(new PersistentHubFixture(output,new ProcessHubInfrastructure())) {}

        internal class ProcessHubInfrastructure : IPersistentActorTestsInfrastructure
        {
            public ProcessHubInfrastructure()
            {
                var processId = Guid.NewGuid();
                ChildId = processId;
                var gotTired = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), processId);
                var coffeMadeEvent = new CoffeMadeEvent(gotTired.FavoriteCoffeMachineId, gotTired.PersonId, null, processId);

                ChildCreateMessage = new MessageMetadataEnvelop<DomainEvent>(gotTired, new MessageMetadata(gotTired.SourceId));
                //TODO: second message will not hit same process as created by previos, 
                //think how to change it. 
                ChildActivateMessage = new MessageMetadataEnvelop<DomainEvent>(coffeMadeEvent,
                                                                               new MessageMetadata(coffeMadeEvent.SourceId));
            }

            Props IPersistentActorTestsInfrastructure.CreateHubProps(ActorSystem system)
            {
                return
                    system.DI().Props<ProcessHubActor<SoftwareProgrammingState>>();
            }

            public object ChildCreateMessage { get; }
            public object ChildActivateMessage { get; }
            public Guid ChildId { get; }
        }
    }
}