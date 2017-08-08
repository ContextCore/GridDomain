using System;
using System.Runtime.Serialization;

namespace GridDomain.EventSourcing
{
    /// <summary>
    ///     DomainException represents an expected error in domain logic,
    ///     not meaning something bad in infrastructure or logic was happened
    /// </summary>
    [Serializable]
    public class DomainException : Exception
    {
        public DomainException() {}

        public DomainException(string message) : base(message) {}

        public DomainException(string message, Exception inner) : base(message, inner) {}

        public DomainException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}