using System;
using GridDomain.CQRS;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandWasNotConfirmedException : Exception
    {
        public ScheduledCommandWasNotConfirmedException(ICommand command)
        {
            Command = command;
        }

        private ICommand Command { get; }
    }
}