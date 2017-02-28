namespace GridDomain.Scheduling.Integration
{
    public class JobSucceeded : JobCompleted
    {
        public JobSucceeded(string name, string group, object message) : base(name, group)
        {
            Message = message;
        }

        public object Message { get; }
    }
}