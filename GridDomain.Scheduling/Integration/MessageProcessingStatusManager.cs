using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class MessageProcessingStatusManager : UntypedActor
    {
        private readonly IPublisher _publisher;
        private IActorRef _quartzJobActorRef;
        public MessageProcessingStatusManager(IPublisher publisher)
        {
            _publisher = publisher;
        }

        protected override void OnReceive(object message)
        {
            if (message is MessageSuccessfullyProcessed || message is MessageProcessingFailed)
            {
                _quartzJobActorRef.Tell(message);
            }
            else
            {
                _quartzJobActorRef = Sender;
                _publisher.Publish(message);
            }
        }
    }
}