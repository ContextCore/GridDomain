namespace GridDomain.Scheduling.Integration
{
    public class JobSucceeded : JobCompleted
    {
        public JobSucceeded(string name, string group):base(name,group)
        {
        }
    }
}