using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface IHandler<in T>
    {
        void Handle(T msg);
    }

    public interface IHandlerWithMetadata<in T> : IHandler<T>
    {
        void Handle(T message, IMessageMetadata metadata = null);
    }
}