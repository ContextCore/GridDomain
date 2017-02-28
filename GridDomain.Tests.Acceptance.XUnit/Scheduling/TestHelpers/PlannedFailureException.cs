using System;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class PlannedFailureException : Exception
    {
        public PlannedFailureException(int timeToOkResponse)
        {
            TimeToOkResponse = timeToOkResponse;
        }

        public int TimeToOkResponse { get; }
    }
}