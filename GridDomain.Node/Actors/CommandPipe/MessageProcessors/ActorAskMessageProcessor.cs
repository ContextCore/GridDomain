using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;

namespace GridDomain.Node.Actors.CommandPipe.MessageProcessors
{
    public class ActorAskMessageProcessor<T>: IMessageProcessor<T>
    {
        private readonly IActorRef _processor;
        private readonly TimeSpan? _defaultTimeout;

        public ActorAskMessageProcessor(IActorRef processor, TimeSpan? defaultTimeout = null)
        {
            _defaultTimeout = defaultTimeout;
            _processor = processor;
        }
        
        public Task<T> Process(IMessageMetadataEnvelop message)
        {
            return _processor.Ask<T>(message, _defaultTimeout);
        }

        Task IMessageProcessor.Process(IMessageMetadataEnvelop message)
        {
            return Process(message);
        }
    }
}