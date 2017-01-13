using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface IHandlerWithMetadata<in T> : IHandler<T>
    {
        Task Handle(T message, IMessageMetadata metadata = null);
    }
}