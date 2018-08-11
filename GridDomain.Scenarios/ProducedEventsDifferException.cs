using System;

namespace GridDomain.Tests.Scenarios {
    public class ProducedEventsDifferException : Exception
    {
        public ProducedEventsDifferException(string message):base(message)
        {
            
        }
    }
}