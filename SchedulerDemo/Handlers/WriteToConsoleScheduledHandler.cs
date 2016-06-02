using GridDomain.Scheduling.Akka;
using SchedulerDemo.Messages;
using SchedulerDemo.ScheduledRequests;

namespace SchedulerDemo.Handlers
{
    public class WriteToConsoleScheduledHandler : ScheduledTaskHandler<WriteToConsoleRequest>
    {
        protected override void Handle(WriteToConsoleRequest request)
        {
            ActorReferences.Writer.Tell(new WriteToConsole(request.TaskId), Self);
        }
    }
}