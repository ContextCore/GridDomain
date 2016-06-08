using GridDomain.CQRS.Messaging;
using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Akka.Messages;
using SchedulerDemo.Messages;
using SchedulerDemo.ScheduledMessages;

namespace SchedulerDemo.Handlers
{
    public class WriteToConsoleScheduledHandler : ScheduledMessageHandler<WriteToConsoleScheduledCommand>
    {
        private readonly IPublisher _publisher;

        public WriteToConsoleScheduledHandler(IPublisher publisher)
        {
            _publisher = publisher;
        }

        protected override void Handle(WriteToConsoleScheduledCommand scheduledCommand)
        {
            _publisher.Publish(new WriteToConsole("scheduled event " + scheduledCommand.TaskId));
            Sender.Tell(new MessageSuccessfullyProcessed(scheduledCommand.TaskId), Self);
        }
    }
}