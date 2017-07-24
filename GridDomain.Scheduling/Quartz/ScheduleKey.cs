namespace GridDomain.Scheduling.Quartz
{
    public class ScheduleKey
    {
        public ScheduleKey(string name, string group, string description = null)
        {
            Name = name;
            Group = group;
            Description = description;
        }

        public string Name { get; private set; }
        public string Group { get; private set; }
        public string Description { get; private set; }

    }
}