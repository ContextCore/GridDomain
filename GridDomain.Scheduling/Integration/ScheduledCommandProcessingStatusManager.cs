using System;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandProcessingStatusManager : ReceiveActor
    {
        private readonly IPublisher _publisher;
        private readonly IActorSubscriber _actorSubscriber;
        private IActorRef _quartzJobActorRef;
        public ScheduledCommandProcessingStatusManager(
            IPublisher publisher,
            IActorSubscriber actorSubscriber)
        {
            _publisher = publisher;
            _actorSubscriber = actorSubscriber;
            Receive<ManageScheduledCommand>(x => Manage(x));
            Receive<CompleteJob>(x =>
            {
                _quartzJobActorRef?.Tell(new object());
            }, x => x.TaskId == TaskId && x.Group == Group);
        }

        public string TaskId { get; private set; }
        public string Group { get; private set; }

        private void Manage(ManageScheduledCommand envelope)
        {
            TaskId = envelope.Command.TaskId;
            Group = envelope.Command.Group;
            _actorSubscriber.Subscribe<CompleteJob>(Self);
            _quartzJobActorRef = Sender;
            _publisher.Publish(new ScheduledCommandProcessingStarted(Guid.Empty, envelope.Command));
        }
    }
}