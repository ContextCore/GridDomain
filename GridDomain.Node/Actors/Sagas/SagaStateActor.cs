using Akka.Actor;
using GridDomain.Configuration;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.Sagas.Messages;

namespace GridDomain.Node.Actors.Sagas
{
    public class SagaStateActor<TState> : AggregateActor<SagaStateAggregate<TState>> where TState : ISagaState
    {
        public SagaStateActor(IAggregateCommandsHandler<SagaStateAggregate<TState>> handler,
                              IPublisher publisher,
                              ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                              IConstructAggregates aggregateConstructor,
                              IActorRef customHandlersActor) : base(handler,
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