using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Configuration.Composition;

namespace GridDomain.Node.Actors.Aggregates {
    class AggregateCommandExecutionContext
    {
        public IAggregate ProducedState;
        public ICommand Command;
        public IMessageMetadata CommandMetadata;
        public IActorRef CommandSender;
        public Exception Exception;
        public IActorRef PersistenceWaiter;

        public void Clear()
        {
            ProducedState = null;
            Command = null;
            CommandMetadata = null;
            CommandSender = null;
            Exception = null;
            PersistenceWaiter = null;
        }
    }
}