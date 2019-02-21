using System;

namespace GridDomain.Scenarios {
    public class ProducedEventsDifferException : Exception
    {
        public ProducedEventsDifferException(string message):base(message)
        {
            
        }
    }
}