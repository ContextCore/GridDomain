namespace GridDomain.Scheduling.Akka.Tasks
{
    public abstract class ProcessScheduledTaskRequest
    {
        public string TaskId { get; private set; }

        protected ProcessScheduledTaskRequest(string taskId)
        {
            TaskId = taskId;
        }

    }
}