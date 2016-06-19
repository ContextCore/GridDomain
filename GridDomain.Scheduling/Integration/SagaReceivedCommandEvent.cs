using GridDomain.CQRS;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class SagaReceivedCommandEvent
    {
        public Command Command { get; }
        public ScheduleKey Key { get; }

        public SagaReceivedCommandEvent(Command command, ScheduleKey key)
        {
            Command = command;
            Key = key;
        }
    }
}