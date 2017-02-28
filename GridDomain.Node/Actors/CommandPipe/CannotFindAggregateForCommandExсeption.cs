using System;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class CannotFindAggregateForCommandExñeption : Exception
    {
        public object Command { get; }
        public Type Topic { get; }

        public CannotFindAggregateForCommandExñeption(object command, Type topic)
        {
            Command = command;
            Topic = topic;
        }
    }
}