using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Logging;

namespace GridDomain.Node
{
    public class DefaultMessageLoggerHandler : IHandler<DomainEvent>,
                                               IHandler<ICommand>,
                                               IHandler<IFault>
    {
        private Task Handle(object msg)
        {
            return Task.CompletedTask;
        }

        public Task Handle(DomainEvent msg)
        {
            return Handle((object)msg);
        }

        public Task Handle(ICommand msg)
        {
            return Handle((object)msg);
        }

        public Task Handle(IFault msg)
        {
            return Handle((object)msg);
        }
    }
}