using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Scheduling.Quartz;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduleCommandExecution
    {
        public ScheduleCommandExecution(ICommand command,
                                        ScheduleKey key,
                                        ExecutionOptions options,
                                        IMessageMetadata commandMetadata=null)
        {
            Command = command;
            Key = key;
            Options = options;
            CommandMetadata = commandMetadata ?? MessageMetadata.Empty;
        }

        public ICommand Command { get; }
        public IMessageMetadata CommandMetadata { get; }
        public ScheduleKey Key { get; }
        public ExecutionOptions Options { get; }
    }
}