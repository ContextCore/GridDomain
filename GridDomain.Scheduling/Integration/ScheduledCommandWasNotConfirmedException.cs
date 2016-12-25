using System;
using GridDomain.CQRS;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandWasNotConfirmedException : Exception
    {
        private ICommand Command { get; }

        public ScheduledCommandWasNotConfirmedException(ICommand command)
        {
            Command = command;
        }
    }
}