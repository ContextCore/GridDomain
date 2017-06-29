using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Tests.Common
{
    public class AnyMessagePublisher
    {
        private readonly CommandPipe _commandPipe;
        private readonly LocalMessagesWaiter<AnyMessagePublisher> _waiter;

        public AnyMessagePublisher(CommandPipe commandPipe, LocalMessagesWaiter<AnyMessagePublisher> waiter)
        {
            _waiter = waiter;
            _commandPipe = commandPipe;
        }

        public async Task<IWaitResult> SendToSagas(DomainEvent message, Guid sagaId, IMessageMetadata metadata = null)
        {
            return await SendToSagas(message.CloneWithSaga(sagaId), metadata);
        }

        public async Task<IWaitResult> SendToSagas(DomainEvent message, IMessageMetadata metadata = null)
        {
            var task = _waiter.Start();

            _commandPipe.SagaProcessor.Tell(new MessageMetadataEnvelop<DomainEvent>(message,
                                                                                    metadata ?? MessageMetadata.Empty));

            return await task;
        }

        public async Task<IWaitResult> SendToSagas(IFault message, IMessageMetadata metadata = null)
        {
            var task = _waiter.Start();

            _commandPipe.SagaProcessor.Tell(new MessageMetadataEnvelop<IFault>(message, metadata ?? MessageMetadata.Empty));

            return await task;
        }
    }
}