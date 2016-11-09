using System;

namespace GridDomain.Node.Actors
{
    public class CannotCreateChildException : Exception
    {
        public object Message { get; }
        public Type ChildActorType { get;  }

        public CannotCreateChildException(object message, Type childActorType)
        {
            Message = message;
            ChildActorType = childActorType;
        }
    }
}