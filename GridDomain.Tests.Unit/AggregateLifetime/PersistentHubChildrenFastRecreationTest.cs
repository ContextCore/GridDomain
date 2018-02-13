using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.PersistentHub;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.AggregateLifetime {
    
    
    public class AggregateHub_children_fast_recreation_tests : NodeTestKit
    {
        public AggregateHub_children_fast_recreation_tests(ITestOutputHelper output)
            : this(new PersistentHubFixture(output,
                                            new AggregatePersistedHubInfrastructure(),
                                            TimeSpan.FromMilliseconds(15),
                                            TimeSpan.FromMilliseconds(0)
                                           ))
        {
            
        }
        
        private AggregateHub_children_fast_recreation_tests(PersistentHubFixture fixture)
            : base(fixture)
        {
        
            var actorOfAsTestActorRef = ActorOfAsTestActorRef<PersistentHubActor>(fixture.Infrastructure.CreateHubProps(Node.System),
                                                                                  "TestHub_" + Guid.NewGuid());
            _hubRef = actorOfAsTestActorRef.Ref;
        }

        private readonly IActorRef _hubRef;

        [Fact]
        public void When_child_revives_monitor_should_be_created_even_on_collision()
        {

            var ChildId = "testChild";
            
            var create = new MessageMetadataEnvelop<ICommand>(new InflateNewBallonCommand(42, ChildId),
                                                              MessageMetadata.New(ChildId, null, null));
            
            _hubRef.Tell(create);

            Task.Run(async () =>
                     {
                         while (true)
                         {
                             var activate = new MessageMetadataEnvelop<ICommand>(new WriteTitleCommand(100, ChildId),
                                                                                 MessageMetadata.New(ChildId, null, null));

                             _hubRef.Tell(activate);
                             await Task.Delay(150);
                         }
                     });

            EventFilter.Exception<InvalidActorNameException>()
                       .Expect(1, TimeSpan.FromSeconds(10), () => { });
        }
    }
}