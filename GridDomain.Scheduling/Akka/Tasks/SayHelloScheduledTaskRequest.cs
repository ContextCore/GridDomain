namespace GridDomain.Scheduling.Akka.Tasks
{
    public class SayHelloScheduledTaskRequest : ProcessScheduledTaskRequest
    {
        public SayHelloScheduledTaskRequest() : base("Say hello")
        {
        }
    }
}