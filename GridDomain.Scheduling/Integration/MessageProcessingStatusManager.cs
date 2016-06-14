using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class MessageProcessingStatusManager : ReceiveActor
    {
        private readonly IPublisher _publisher;
        private IActorRef _quartzJobActorRef;
        public MessageProcessingStatusManager(IPublisher publisher)
        {
            _publisher = publisher;
            Receive<ManageMessage>(x => Manage(x));
            Receive<IMessageProcessingStatusChanged>(x => StatusChanged(x));
        }

        private void StatusChanged(IMessageProcessingStatusChanged messageProcessingStatusChanged)
        {
            _quartzJobActorRef?.Tell(messageProcessingStatusChanged);
        }

        private void Manage(ManageMessage envelope)
        {
            _quartzJobActorRef = Sender;
            envelope.Command.Manager = Self;
            _publisher.Publish(envelope.Command);
        }

    }
}