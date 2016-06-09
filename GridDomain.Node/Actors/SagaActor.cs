using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Persistence;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Node.Actors
{
    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TSagaState"></typeparam>
    /// <typeparam name="TStartMessage"></typeparam>
    public class SagaActor<TSaga, TSagaState, TStartMessage> :
        ReceivePersistentActor where TSaga : IDomainSaga
        where TSagaState : AggregateBase
        where TStartMessage : DomainEvent
    {
        private readonly IPublisher _publisher;
        private readonly ISagaFactory<TSaga, TStartMessage> _sagaStarter;
        public TSaga Saga;
        private readonly ISagaFactory<TSaga, TSagaState> _sagaFactory;
        private readonly IEmptySagaFactory<TSaga> _emptySagaFactory;

        public SagaActor(ISagaFactory<TSaga, TStartMessage> sagaStarter,
                         ISagaFactory<TSaga, TSagaState> sagaFactory,
                         IEmptySagaFactory<TSaga> emptySagaFactory,
                         IPublisher publisher)
        {
            _emptySagaFactory = emptySagaFactory;
            _sagaStarter = sagaStarter;
            _sagaFactory = sagaFactory;
            _publisher = publisher;
            Saga = _emptySagaFactory.Create();

            Command<DomainEvent>(cmd =>
            {
                var startMessage = cmd as TStartMessage;
                if (startMessage != null)
                {
                    Saga = _sagaStarter.Create(startMessage);
                }
                
                Saga.Transit(cmd);

                foreach(var msg in Saga.CommandsToDispatch)
                        _publisher.Publish(msg);

                var sagaStateChangeEvents = Saga.StateAggregate.GetUncommittedEvents().Cast<object>();
                PersistAll(sagaStateChangeEvents, e => _publisher.Publish(e));

                Saga.ClearCommandsToDispatch();
                Saga.StateAggregate.ClearUncommittedEvents();
            });

            Recover<SnapshotOffer>(offer => Saga = _sagaFactory.Create((TSagaState) offer.Snapshot));
            Recover<DomainEvent>(e => Saga.StateAggregate.ApplyEvent(e));
        }

        public override string PersistenceId => Self.Path.Name;
    }
}