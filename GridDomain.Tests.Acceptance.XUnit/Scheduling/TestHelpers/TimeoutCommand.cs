using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class TimeoutCommand : Command
    {
        public string Text { get; private set; }
        public TimeSpan Timeout { get; private set; }

        public TimeoutCommand(string text, TimeSpan timeout) : base(Guid.NewGuid())
        {
            Text = text;
            Timeout = timeout;
        }
    }
}