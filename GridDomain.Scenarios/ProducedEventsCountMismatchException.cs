using System;

namespace GridDomain.Tests.Scenarios {
    public class ProducedEventsCountMismatchException : Exception
    {
        public ProducedEventsCountMismatchException(string message) :base(message)
        {
            
        }
    }
}