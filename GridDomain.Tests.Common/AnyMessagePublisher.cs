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

        public Task<IWaitResult> SendToProcessManagers(DomainEvent message, Guid processId, IMessageMetadata metadata = null)
        {
            return SendToProcessManagers(message.CloneForProcess(processId), metadata);
        }

        public Task<IWaitResult> SendToProcessManagers(DomainEvent message, IMessageMetadata metadata = null)
        {
            var task = _waiter.Start();

            _commandPipe.ProcessesPipeActor.Tell(new MessageMetadataEnvelop<DomainEvent>(message,
                                                                                    metadata ?? MessageMetadata.Empty));

            return task;
        }

        public Task<IWaitResult> SendToProcessManagers(IFault message, IMessageMetadata metadata = null)
        {
            var task = _waiter.Start();

            _commandPipe.ProcessesPipeActor.Tell(new MessageMetadataEnvelop<IFault>(message, metadata ?? MessageMetadata.Empty));

            return task;
        }
    }
}