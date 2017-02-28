namespace GridDomain.Scheduling.Integration
{
    public class JobCompleted
    {
        public JobCompleted(string name, string group)
        {
            Name = name;
            Group = group;
        }

        public string Name { get; }
        public string Group { get; }
    }
}