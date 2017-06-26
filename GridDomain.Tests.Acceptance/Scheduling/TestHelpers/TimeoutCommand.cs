using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TimeoutCommand : Command
    {
        public TimeoutCommand(string text, TimeSpan timeout) : base(Guid.NewGuid())
        {
            Text = text;
            Timeout = timeout;
        }

        public string Text { get; private set; }
        public TimeSpan Timeout { get; private set; }
    }
}