using System;
using GridDomain.CQRS;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ExecutionOptions
    {
        public DateTime RunAt { get; private set; }
        public TimeSpan Timeout { get; private set; }

        public ExecutionOptions(DateTime runAt, TimeSpan timeout = default(TimeSpan))
        {
            RunAt = runAt;
            Timeout = timeout == default(TimeSpan) ? TimeSpan.FromMinutes(1) : timeout;
        }
    }

    public class ScheduleKey
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Group { get; private set; }
        public string Description { get; private set; }

        public ScheduleKey(Guid id, string name, string group, string description = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Name = name;
            Group = @group;
            Description = description;
        }

        public static ScheduleKey For(Command cmd, Guid id = default(Guid), string group = null, string description = null)
        {
            var commandType = cmd.GetType();
            id = id == default(Guid) ? Guid.NewGuid() : id;
            group = group ?? commandType.Namespace;
            var name = $"{commandType.Name} #  {id}";
            description = description ?? $"{name} # {group}";
            return new ScheduleKey(id, name, group, description);
        }
    }

    public class Schedule
    {
        public Command Command { get; }
        public ScheduleKey Key { get; }
        public ExecutionOptions Options { get; }

        public Schedule(Command command, ScheduleKey key, ExecutionOptions options)
        {
            Command = command;
            Key = key;
            Options = options;
        }
    }
}