using Akka.Actor;
using CommonDomain.Persistence;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Node.Actors
{
    public class SagaStateActor<TState> : AggregateActor<SagaStateAggregate<TState>> where TState : ISagaState
    {
        public SagaStateActor(IAggregateCommandsHandler<SagaStateAggregate<TState>> handler,
                              IActorRef schedulerActorRef,
                              IPublisher publisher,
                              ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                              IConstructAggregates aggregateConstructor,
                              IActorRef customHandlersActor) : base(handler,
                                                                    schedulerActorRef,
                                                                    publisher,
                                                                    snapshotsPersistencePolicy,
                                                                    aggregateConstructor,
                                                                    customHandlersActor)
        {
        }

        protected override void AwaitingCommandBehavior()
        {
            Command<GetSagaState>(c => Sender.Tell(new SagaState<TState>(State.State)));
            base.AwaitingCommandBehavior();
        }
      
    }
}