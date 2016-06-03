using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class JobStatusManager : UntypedActor
    {
        private readonly IPublisher _publisher;
        private IActorRef _quartzJobActorRef;
        public JobStatusManager(IPublisher publisher)
        {
            _publisher = publisher;
        }

        protected override void OnReceive(object message)
        {
            if (message is TaskProcessed || message is TaskProcessingFailed)
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