using System;
using System.Collections.Generic;
using GridDomain.Aggregates;

namespace GridDomain.Scenarios.Builders {
    public class CommandsBelongToDifferentAggregateTypesException : Exception
    {
        public IReadOnlyCollection<ICommand> Commands { get; }

        public CommandsBelongToDifferentAggregateTypesException(IReadOnlyCollection<ICommand> commands)
        {
            Commands = commands;
        }
    }
}