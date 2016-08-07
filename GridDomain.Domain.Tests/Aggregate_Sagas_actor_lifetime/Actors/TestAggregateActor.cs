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

    internal class ChildCreated
    {
        public object Id { get; set; }

        public ChildCreated(object id)
        {
            Id = id;
        }
    }



    internal class ChildTerminated
    {
        public Guid Id { get; set; }

        public ChildTerminated(Guid id)
        {
            Id = id;
        }
    }

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

        protected override void PreStart()
        {
            base.PreStart();
            _observer.Tell(new ChildCreated(Id));
        }



        protected override void Shutdown()
        {
            _observer.Tell(new ChildTerminated(Id));
            base.Shutdown();
        }

    }
}