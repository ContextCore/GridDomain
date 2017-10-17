using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.ProcessManagers.DomainBind;

namespace GridDomain.Configuration.MessageRouting
{
    public interface IMessagesRouter
    {
        Task RegisterAggregate(IAggregateCommandsHandlerDescriptor descriptor);

        Task RegisterProcess(IProcessDescriptor processDescriptor, string name = null);

        Task RegisterSyncHandler<TMessage, THandler>() where THandler : IHandler<TMessage>
                                                       where TMessage : class, IHaveProcessId, IHaveId;
        Task RegisterFireAndForgetHandler<TMessage, THandler>() where THandler : IHandler<TMessage>
                                                                where TMessage : class, IHaveProcessId, IHaveId;
    }
}