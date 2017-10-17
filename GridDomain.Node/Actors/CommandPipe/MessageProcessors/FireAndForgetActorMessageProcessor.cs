using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node.Actors.Aggregates.Messages;

namespace GridDomain.Node.Actors.CommandPipe.MessageProcessors
{
    public class FireAndForgetActorMessageProcessor : IMessageProcessor
    {
        public FireAndForgetActorMessageProcessor(IActorRef processor)
        {
            ActorRef = processor;
        }

        public Task Process(IMessageMetadataEnvelop message)
        {
            ActorRef.Tell(message);
            return Task.CompletedTask;
        }

        private IActorRef ActorRef { get; }
    }
}