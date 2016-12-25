using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface IHandlerWithMetadata<in T> : IHandler<T>
    {
        void Handle(T message, IMessageMetadata metadata = null);
    }
}