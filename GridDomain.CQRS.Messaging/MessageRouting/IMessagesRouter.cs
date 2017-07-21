using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.ProcessManagers.DomainBind;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IMessagesRouter
    {
        Task RegisterAggregate(IAggregateCommandsHandlerDescriptor descriptor);

        Task RegisterProcess(IProcessManagerDescriptor processManagerDescriptor, string name = null);

        Task RegisterSyncHandler<TMessage, THandler>() where THandler : IHandler<TMessage>
                                                       where TMessage : class, IHaveProcessId, IHaveId;
        Task RegisterFireAndForgetHandler<TMessage, THandler>() where THandler : IHandler<TMessage>
                                                                where TMessage : class, IHaveProcessId, IHaveId;
        Task RegisterParralelHandler<TMessage, THandler>() where THandler : IHandler<TMessage>
                                                           where TMessage : class, IHaveProcessId, IHaveId;
    }
}