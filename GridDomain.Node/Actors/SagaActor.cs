using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Persistence;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{

    public interface ISagaFactory<TSaga,TStartMessage> where TSaga : IDomainSaga
    {
        TSaga Create(TStartMessage message);
    }

    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public class SagaActor<TSaga,TSagaState,TStartMessage>:
        ReceivePersistentActor where TSaga : IDomainSaga
                               where TSagaState : AggregateBase
    {
        private readonly IPublisher _publisher;
        private readonly ISagaFactory<TSaga, TStartMessage> _sagaFactory;
        private TSaga Saga;
        public SagaActor(ISagaFactory<TSaga,TStartMessage> sagaFactory,
                         IPublisher publisher)
        {
            _sagaFactory = sagaFactory;
            _publisher = publisher;
            PersistenceId = Self.Path.Name;

            CommandAny(cmd =>
            {
                if (cmd is TStartMessage) return;
                dynamic saga = Saga;
                saga.Handle(cmd);
                PersistAll(Saga.MessagesToDispatch, e => _publisher.Publish(e));
                Saga.MessagesToDispatch.Clear();
            });
            Command<TStartMessage>(c =>
            {
                Saga = _sagaFactory.Create(c);
            });
            
            Recover<SnapshotOffer>(offer => Saga.StateAggregate = (AggregateBase)offer.Snapshot);
            Recover<DomainEvent>(e => Saga.StateAggregate.ApplyEvent(e));
        }

        public override string PersistenceId { get; }
    }
}
