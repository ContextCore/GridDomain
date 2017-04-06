using System;
using System.Diagnostics;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ExecutionOptions
    {
        public ExecutionOptions(DateTime runAt,
                                Type succesEventType,
                                Guid successMessageId,
                                string messageIdFieldName,
                                TimeSpan? timeout = null) 
        {
            RunAt = runAt;
            SuccesEventType = succesEventType;
            Timeout = timeout ?? TimeSpan.FromMinutes(1);
            SuccessMessageId = successMessageId;
            MessageIdFieldName = messageIdFieldName;
        }

        public static ExecutionOptions ForCommand(DateTime runAt, Type succesEventType, TimeSpan? timeout = null)
        {
            return new ExecutionOptions(runAt, succesEventType, Guid.Empty, string.Empty, timeout);
        }

        public Type SuccesEventType { get; }
        public DateTime RunAt { get; }
        public TimeSpan Timeout { get; }
        public Guid SuccessMessageId { get; }
        public string MessageIdFieldName { get; }
    }
}