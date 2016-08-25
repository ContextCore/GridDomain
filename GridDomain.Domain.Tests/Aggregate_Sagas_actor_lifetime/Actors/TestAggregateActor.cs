using System;
using Akka;
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
    //TODO : replace with HealthCheck \ HealthStatus
    class Ping
    {
        public object Payload { get; }

        public Ping(object payload)
        {
            Payload = payload;
        }
    }

    class Pong
    {
        public Pong(object payload)
        {
            Payload = payload;
        }

        public object Payload { get; }
    }

    class TestAggregateActor : AggregateActor<SampleAggregate>
    {
        public TestAggregateActor(IAggregateCommandsHandler<SampleAggregate> handler, 
                                  AggregateFactory factory, 
                                  TypedMessageActor<ScheduleCommand> schedulerActorRef, 
                                  TypedMessageActor<Unschedule> unscheduleActorRef, 
                                  IPublisher publisher) : base(handler, factory, schedulerActorRef, unscheduleActorRef, publisher)
        {
        }

        protected override bool Receive(object message)
        {
            //echo for testing purpose
           message.Match().With<Ping>(m => Sender.Tell(new Pong(m.Payload)));
           return base.Receive(message);
        }

        
    }
}