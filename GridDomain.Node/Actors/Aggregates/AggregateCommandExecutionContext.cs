using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.Aggregates {
    class AggregateCommandExecutionContext<TAggregate> where TAggregate : EventSourcing.Aggregate
    {
        public TAggregate ProducedState;
        public ICommand Command;
        public IMessageMetadata CommandMetadata;
        public readonly List<object> MessagesToProject = new List<object>();

        public void Clear()
        {
            ProducedState = null;
            Command = null;
            CommandMetadata = null;
            MessagesToProject.Clear();;
        }
    }
}