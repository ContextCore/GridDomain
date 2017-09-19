using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node.Actors.Aggregates.Messages;

namespace GridDomain.Node.Actors.CommandPipe.MessageProcessors {


    public class CommandProcessor : IMessageProcessor<CommandExecuted>
    {
        private readonly IActorRef _aggregateHubActor;

        public CommandProcessor(IActorRef aggregateHubActor)
        {
            _aggregateHubActor = aggregateHubActor;
        }
        public Task<CommandExecuted> Process(object message, ref Task workInProgress)
        {
            var wip = _aggregateHubActor.Ask<CommandExecuted>(message);
            workInProgress = wip;
            return wip;
        }
    }

    public class FireAndForgetMessageProcessor : IMessageProcessor
    {
        public FireAndForgetMessageProcessor(IActorRef processor)
        {
            ActorRef = processor;
        }

        public Task Process(object message, ref Task workInProgress)
        {
            ActorRef.Tell(message);
            return workInProgress ?? Task.CompletedTask;
        }

        public IActorRef ActorRef { get; }
    }
}