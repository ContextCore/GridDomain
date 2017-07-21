using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.MessageProcessors {


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