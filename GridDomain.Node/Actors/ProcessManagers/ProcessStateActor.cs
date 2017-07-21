using Akka.Actor;
using GridDomain.Configuration;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Node.Actors.ProcessManagers
{
    public class ProcessStateActor<TState> : AggregateActor<ProcessStateAggregate<TState>> where TState : IProcessState
    {
        public ProcessStateActor(IAggregateCommandsHandler<ProcessStateAggregate<TState>> handler,
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
            Command<GetProcessState>(c => Sender.Tell(new ProcesStateMessage<TState>(State.State)));
            base.AwaitingCommandBehavior();
        }
      
    }
}