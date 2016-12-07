using System;
using GridDomain.CQRS;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandWasNotConfirmedException : Exception
    {
        private Command Command { get; }

        public ScheduledCommandWasNotConfirmedException(Command command)
        {
            Command = command;
        }
    }
}