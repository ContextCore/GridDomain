using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node
{
    public class DefaultMessageLoggerHandler : IHandler<DomainEvent>,
                                               IHandler<ICommand>,
                                               IHandler<IFault>
    {
        public Task Handle(DomainEvent msg, IMessageMetadata metadata)
        {
            return Handle((object) msg);
        }

        public Task Handle(ICommand msg, IMessageMetadata metadata)
        {
            return Handle((object) msg);
        }

        public Task Handle(IFault msg, IMessageMetadata metadata)
        {
            return Handle((object) msg);
        }

        private Task Handle(object msg)
        {
            return Task.CompletedTask;
        }
    }
}