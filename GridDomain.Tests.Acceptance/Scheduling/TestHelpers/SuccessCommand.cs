using System;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class SuccessCommand : Command<TestAggregate>
    {
        public SuccessCommand(string text) : base(Guid.NewGuid().ToString())
        {
            Text = text;
        }

        public string Text { get; private set; }
    }
}