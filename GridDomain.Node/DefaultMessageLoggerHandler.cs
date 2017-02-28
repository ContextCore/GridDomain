using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node
{
    public class DefaultMessageLoggerHandler : IHandler<DomainEvent>,
                                               IHandler<ICommand>,
                                               IHandler<IFault>
    {
        public Task Handle(DomainEvent msg)
        {
            return Handle((object) msg);
        }

        public Task Handle(ICommand msg)
        {
            return Handle((object) msg);
        }

        public Task Handle(IFault msg)
        {
            return Handle((object) msg);
        }

        private Task Handle(object msg)
        {
            return Task.CompletedTask;
        }
    }
}