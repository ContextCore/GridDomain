using GridDomain.CQRS;

namespace SchedulerDemo.Events
{
    public class WriteToConsoleScheduledCommand : Command
    {
        public string Text { get; private set; }

        public WriteToConsoleScheduledCommand(string text)
        {
            Text = text;
        }
    }
}