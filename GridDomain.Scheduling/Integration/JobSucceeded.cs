namespace GridDomain.Scheduling.Integration
{
    public class JobSucceeded : JobCompleted
    {
        public object Message { get; }
        public JobSucceeded(string name, string group, object message):base(name,group)
        {
            Message = message;
        }
    }
}