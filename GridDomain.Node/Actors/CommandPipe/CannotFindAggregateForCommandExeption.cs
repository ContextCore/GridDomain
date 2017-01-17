using System;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class CannotFindAggregateForCommandExeption : Exception
    {
        public object Command { get; }
        public Type Topic { get; }

        public CannotFindAggregateForCommandExeption(object command, Type topic)
        {
            this.Command = command;
            this.Topic = topic;
        }
    }
}