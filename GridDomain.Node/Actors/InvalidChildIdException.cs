using System;

namespace GridDomain.Node.Actors
{
    public class InvalidChildIdException : Exception
    {
        public object Message { get; }

        public InvalidChildIdException(object message):base("Child id should not be empty")
        {
            Message = message;
        }
    }
}