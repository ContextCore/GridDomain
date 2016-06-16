using System;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class MessageProcessingStatusManager : ReceiveActor
    {
        private readonly IPublisher _publisher;
        private readonly IActorSubscriber _actorSubscriber;
        private IActorRef _quartzJobActorRef;
        public MessageProcessingStatusManager(
            IPublisher publisher,
            IActorSubscriber actorSubscriber)
        {
            _publisher = publisher;
            _actorSubscriber = actorSubscriber;
            Receive<ManageMessage>(x => Manage(x));
            Receive<IMessageProcessingStatusChanged>(x => StatusChanged(x));
            Receive<CompleteJob>(x =>
            {
                _quartzJobActorRef?.Tell(new MessageSuccessfullyProcessed(x.TaskId));
            }, x => x.TaskId == TaskId && x.Group == Group);
        }

        public string TaskId { get; private set; }
        public string Group { get; private set; }

        private void StatusChanged(IMessageProcessingStatusChanged messageProcessingStatusChanged)
        {
            _quartzJobActorRef?.Tell(messageProcessingStatusChanged);
        }

        private void Manage(ManageMessage envelope)
        {
            TaskId = envelope.Command.TaskId;
            Group = envelope.Command.Group;
            _actorSubscriber.Subscribe<CompleteJob>(Self);
            _quartzJobActorRef = Sender;
            _publisher.Publish(new ScheduledMessageProcessingStarted(Guid.Empty, envelope.Command));
        }
    }
}