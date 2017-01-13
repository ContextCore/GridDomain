using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IProjectionGroup : IProjectionGroupDescriptor, IHandlerWithMetadata<object>
    {
        Task Project(object message, IMessageMetadata metadata);
    }

}