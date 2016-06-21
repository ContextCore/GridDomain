using System;
using GridDomain.CQRS;

namespace GridDomain.Scheduling.Akka.Messages
{
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
}