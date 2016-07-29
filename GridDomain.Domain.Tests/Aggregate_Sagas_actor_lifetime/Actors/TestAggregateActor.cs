using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Tests.SampleDomain;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    class TestAggregateActor : AggregateActor<SampleAggregate>
    {
        public TestAggregateActor(IAggregateCommandsHandler<SampleAggregate> handler, AggregateFactory factory, TypedMessageActor<ScheduleCommand> schedulerActorRef, TypedMessageActor<Unschedule> unscheduleActorRef, IPublisher publisher) : base(handler, factory, schedulerActorRef, unscheduleActorRef, publisher)
        {
        }

        protected override void PreStart()
        {
            base.PreStart();
            PersistentHubTestsStatus.ChildExistence.Add(Id);
        }

        protected override void Shutdown()
        {
            PersistentHubTestsStatus.ChildExistence.Remove(Id);
            base.Shutdown();
        }
    }
}