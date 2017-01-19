using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using Akka.Actor;

namespace GridDomain.Tests.Framework
{
    public class AnyMessagePublisher 
    {
        private readonly CommandPipeBuilder _commandPipe;
        private readonly LocalMessagesWaiter<AnyMessagePublisher> _waiter;

        public AnyMessagePublisher(CommandPipeBuilder commandPipe, LocalMessagesWaiter<AnyMessagePublisher> waiter)
        {
            _waiter = waiter;
            _commandPipe = commandPipe;
        }

  
        public async Task<IWaitResults> SendToSaga(DomainEvent message, Guid sagaId, IMessageMetadata metadata=null)
        {
            return await SendToSaga(message.CloneWithSaga(sagaId), metadata);
        }

        public async Task<IWaitResults> SendToSaga(DomainEvent message, IMessageMetadata metadata = null)
        {
            var task = _waiter.Start();

            _commandPipe.SagasProcessActor.Tell(new MessageMetadataEnvelop<DomainEvent[]>(new[] { message }, metadata ?? MessageMetadata.Empty));

            return await task;
        }
    }
}