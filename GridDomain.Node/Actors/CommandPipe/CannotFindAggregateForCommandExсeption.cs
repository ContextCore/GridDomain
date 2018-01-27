using System;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class CannotFindAggregateForCommandException : Exception
    {
        public CannotFindAggregateForCommandException()
        {
            
        }
        public CannotFindAggregateForCommandException(object command, Type topic)
        {
            Command = command;
            Topic = topic;
        }

        public object Command { get; }
        public Type Topic { get; }
    }
}