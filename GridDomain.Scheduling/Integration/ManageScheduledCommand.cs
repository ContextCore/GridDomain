using GridDomain.CQRS;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class StartSchedulerSaga
    {
        public Command Command { get; }
        public ScheduleKey Key { get; }

        public StartSchedulerSaga(Command command, ScheduleKey key)
        {
            Command = command;
            Key = key;
        }
    }
}