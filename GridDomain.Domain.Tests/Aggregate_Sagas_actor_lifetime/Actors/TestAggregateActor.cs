using System;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Tests.SampleDomain;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{

    class TestAggregateActor : AggregateActor<SampleAggregate>
    {
        private readonly IActorRef _observer;

        public TestAggregateActor(IAggregateCommandsHandler<SampleAggregate> handler, 
                                  AggregateFactory factory, 
                                  TypedMessageActor<ScheduleCommand> schedulerActorRef, 
                                  TypedMessageActor<Unschedule> unscheduleActorRef, 
                                  IPublisher publisher,
                                  IActorRef observer) : base(handler, factory, schedulerActorRef, unscheduleActorRef, publisher)
        {
            _observer = observer;
        }

        protected override bool Receive(object message)
        {
            _observer.Tell(message); //echo for testing purpose
            return base.Receive(message);
        }
    }
}