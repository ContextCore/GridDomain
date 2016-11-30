using System;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ExtendedExecutionOptions : ExecutionOptions
    {
        public Guid SuccessMessageId { get; }
        public string MessageIdFieldName { get; }

        public int RepeatCount { get; }
        public TimeSpan RepeatInterval { get; }
        public ExtendedExecutionOptions(DateTime runAt,
                                        Type succesEventType, 
                                        Guid successMessageId, 
                                        string messageIdFieldName, 
                                        TimeSpan? timeout = null,
                                        int? repeatCount = null,
                                        TimeSpan? repeatInteral = null): base(runAt, succesEventType, timeout)
        {
            SuccessMessageId = successMessageId;
            MessageIdFieldName = messageIdFieldName;
            RepeatInterval = repeatInteral ?? TimeSpan.FromMinutes(10);
            RepeatCount = repeatCount ?? 3;
        }
    }
}