using System;

namespace GridDomain.Tests.Common {
    public class ProducedEventsDifferException : Exception
    {
        public ProducedEventsDifferException(string message):base(message)
        {
            
        }
    }
}