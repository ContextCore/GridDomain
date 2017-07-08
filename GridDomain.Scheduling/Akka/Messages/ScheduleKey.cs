using System;
using GridDomain.CQRS;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduleKey
    {
        public ScheduleKey(Guid id, string name, string group, string description = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Name = name;
            Group = group;
            Description = description;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Group { get; private set; }
        public string Description { get; private set; }

    }
}