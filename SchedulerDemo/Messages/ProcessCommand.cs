namespace SchedulerDemo.Messages
{
    public class ProcessCommand
    {
        public string Command { get; }

        public ProcessCommand(string command)
        {
            Command = command;
        }
    }
}