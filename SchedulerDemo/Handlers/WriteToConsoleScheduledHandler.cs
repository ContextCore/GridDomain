using GridDomain.CQRS.Messaging;
using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Akka.Messages;
using SchedulerDemo.Messages;
using SchedulerDemo.ScheduledMessages;

namespace SchedulerDemo.Handlers
{
    public class WriteToConsoleScheduledHandler : ScheduledMessageHandler<WriteToConsoleScheduledMessage>
    {
        private readonly IPublisher _publisher;

        public WriteToConsoleScheduledHandler(IPublisher publisher)
        {
            _publisher = publisher;
        }

        protected override void Handle(WriteToConsoleScheduledMessage scheduledMessage)
        {
            _publisher.Publish(new WriteToConsole("scheduled event " + scheduledMessage.TaskId));
            Sender.Tell(new MessageSuccessfullyProcessed(scheduledMessage.TaskId), Self);
        }
    }
}