using Akka.Persistence;
using CommonDomain.Core;
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
    {
        private readonly IPublisher _publisher;
        private readonly ISagaFactory<TSaga, TStartMessage> _sagaStarter;
        public TSaga Saga;
        private readonly ISagaFactory<TSaga, TSagaState> _sagaFactory;

        public SagaActor(ISagaFactory<TSaga, TStartMessage> sagaStarter,
                         ISagaFactory<TSaga, TSagaState> sagaFactory,
                         IPublisher publisher)
        {
            _sagaStarter = sagaStarter;
            _sagaFactory = sagaFactory;
            _publisher = publisher;
            PersistenceId = Self.Path.Name;

            CommandAny(cmd =>
            {
                if (cmd is TStartMessage)
                {
                    var startMessage = (TStartMessage)cmd;
                    Saga = _sagaStarter.Create(startMessage);
                    return;
                };

                dynamic saga = Saga;
                saga.Handle(cmd);
                PersistAll(Saga.MessagesToDispatch, e => _publisher.Publish(e));
                Saga.MessagesToDispatch.Clear();
            });
            Recover<SnapshotOffer>(offer => Saga = _sagaFactory.Create((TSagaState) offer.Snapshot));
            Recover<DomainEvent>(e => Saga.StateAggregate.ApplyEvent(e));
        }

        public override string PersistenceId { get; }
    }
}