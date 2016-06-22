using System;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledSagaCreator<TSuccessEvent> : ReceiveActor where TSuccessEvent : DomainEvent
    {
        private readonly IPublisher _publisher;
        private IActorRef _quartzJobActorRef;
        public ScheduledSagaCreator(
            IPublisher publisher,
            IActorSubscriber actorSubscriber)
        {
            _publisher = publisher;
            actorSubscriber.Subscribe<SagaCreatedEvent<ScheduledCommandProcessingSaga.States>>(Self);
            Receive<ManageScheduledCommand>(x => Manage(x));
            Receive<SagaCreatedEvent<ScheduledCommandProcessingSaga.States>>(x =>
            {
                _quartzJobActorRef?.Tell(new object());
            }, x => x.SagaId == SagaId);
        }

        public Guid SagaId { get; private set; }

        private void Manage(ManageScheduledCommand envelope)
        {
            SagaId = envelope.Key.Id;
            _quartzJobActorRef = Sender;
            _publisher.Publish(ScheduledCommandProcessingStarted.Create<TSuccessEvent>(envelope.Command, envelope.Key));
        }
    }
}