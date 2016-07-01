using System;

namespace GridDomain.CQRS.Messaging
{
    public interface IServiceLocator
    {
        T Resolve<T>();
        object Resolve(Type type);
    }
}