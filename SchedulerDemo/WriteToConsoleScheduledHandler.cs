using System.Threading.Tasks;
using GridDomain.Scheduling.Akka;

namespace SchedulerDemo
{
    public class WriteToConsoleScheduledHandler : ScheduledTaskHandler<WriteToConsoleRequest>
    {
        protected override void Handle(WriteToConsoleRequest request)
        {
            ActorReferences.Writer.Tell(new WriteToConsole(request.TaskId), Self);
        }
    }
}