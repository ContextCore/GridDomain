using Akka.Actor;
using GridDomain.Aggregates;

namespace GridDomain.Node.Akka.Actors.Aggregates {
    class AggregateCommandExecutionContext
    {
        public ICommand Command;
        public IMessageMetadata CommandMetadata;
        public IActorRef CommandSender;

        public void Clear()
        {
            Command = null;
            CommandMetadata = null;
            CommandSender = null;
        }
    }
}