using GridDomain.CQRS;

namespace SchedulerDemo.Messages
{
    public class WriteToConsole : Command
    { 
        public string Text { get; }
        public string PartToHighlight { get; set; }

        public WriteToConsole(string text, string partToHighlight = null)
        {
            Text = text;
            PartToHighlight = partToHighlight;
        }
    }
}