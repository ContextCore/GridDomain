using GridDomain.CQRS;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class SuccessCommand : Command
    {
        public string Text { get; private set; }

        public SuccessCommand(string text)
        {
            Text = text;
        }
    }
}