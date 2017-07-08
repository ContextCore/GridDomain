using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.Aggregate {
    class AggregateCommandExecutionContext<TAggregate> where TAggregate : EventSourcing.Aggregate
    {
        public TAggregate ProducedState;
        public ICommand Command;
        public IMessageMetadata CommandMetadata;
        public Func<Task> AfterPersistAction;
        public object[] MessagesToProject;

        public void Clear()
        {
            ProducedState = null;
            Command = null;
            CommandMetadata = null;
            AfterPersistAction = null;
            MessagesToProject = null;
        }
    }
}