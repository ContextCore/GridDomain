using System;
using Akka.DI.Core;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.DependencyInjection.Infrastructure;
using NUnit.Framework;

namespace GridDomain.Tests.DependencyInjection
{
    [TestFixture]
    public class AggregateActor_can_be_created_with_iServiceLocator : AggregatesDI
    {

        [Test]
        public void AggregateActor_can_be_created_with_iServiceLocator_injected()
        {
            var actorRef = GridNode.System.ActorOf(GridNode.System.DI().Props<AggregateActor<TestAggregate>>(),
                AggregateActorName.New<TestAggregate>(Guid.NewGuid()).Name);
            Assert.NotNull(actorRef);
        }
    }
}