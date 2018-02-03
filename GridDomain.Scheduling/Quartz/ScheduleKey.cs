namespace GridDomain.Scheduling.Quartz
{
    public class ScheduleKey
    {
        private readonly string _shortDescription;

        public ScheduleKey(string name, string group, string description = null)
        {
            Name = name;
            Group = group;
            Description = description;
            _shortDescription = Group +"_"+ Name;
        }

        public string Name { get; private set; }
        public string Group { get; private set; }
        public string Description { get; private set; }

        public override string ToString() => _shortDescription;
    }
}