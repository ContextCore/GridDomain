using GridDomain.CQRS;

namespace GridDomain.Scheduling.Integration
{
    public class JobCompleted
    { 
        public string Name { get; }
        public string Group { get; }

        public JobCompleted(string name, string group)
        {
            Name = name;
            Group = group;
        }
    }
}