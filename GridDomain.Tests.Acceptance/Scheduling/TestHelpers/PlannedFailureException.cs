using System;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class PlannedFailureException : Exception
    {
        public int TimeToOkResponse { get; }

        public PlannedFailureException(int timeToOkResponse)
        {
            TimeToOkResponse = timeToOkResponse;
        }
    }
}