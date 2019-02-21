using System;

namespace GridDomain.Scenarios {
    public class ProducedEventsCountMismatchException : Exception
    {
        public ProducedEventsCountMismatchException(string message) :base(message)
        {
            
        }
    }
}