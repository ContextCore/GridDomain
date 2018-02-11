using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class SuccessCommand : Command
    {
        public SuccessCommand(string text) : base(Guid.NewGuid().ToString())
        {
            Text = text;
        }

        public string Text { get; private set; }
    }
}