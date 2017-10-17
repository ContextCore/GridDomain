using System;

namespace GridDomain.Tests.Common {
    public class ProducedEventsCountMismatchException : Exception
    {
        public ProducedEventsCountMismatchException(string message) :base(message)
        {
            
        }
    }
}