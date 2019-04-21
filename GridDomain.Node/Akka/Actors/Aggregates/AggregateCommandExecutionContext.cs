using System.Collections.Generic;
using Akka.Actor;
using GridDomain.Aggregates;

namespace GridDomain.Node.Akka.Actors.Aggregates {
    class AggregateCommandExecutionContext
    {
        public ICommand Command;
        public IMessageMetadata CommandMetadata;
        public IActorRef CommandSender;
        public IReadOnlyCollection<IDomainEvent> ProducedEvents;
        public bool EventsPersisted;
        public bool IsWaitingForConfirmation { get; set; }

        public void Clear()
        {
            Command = null;
            CommandMetadata = null;
            CommandSender = null;
            ProducedEvents = null;
            EventsPersisted = false;
            IsWaitingForConfirmation = false;
        }
    }
}