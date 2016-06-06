using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Akka.Messages;
using SchedulerDemo.Messages;
using SchedulerDemo.ScheduledRequests;

namespace SchedulerDemo.Handlers
{
    public class WriteToConsoleScheduledHandler : ScheduledMessageHandler<WriteToConsoleScheduledMessage>
    {
        protected override void Handle(WriteToConsoleScheduledMessage scheduledMessage)
        {
            ActorReferences.Writer.Tell(new WriteToConsole("scheduled event " + scheduledMessage.TaskId), Self);
            Sender.Tell(new MessageSuccessfullyProcessed(scheduledMessage.TaskId), Self);
        }
    }
}