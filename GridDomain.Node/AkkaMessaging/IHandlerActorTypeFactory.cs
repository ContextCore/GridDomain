using System;

namespace GridDomain.Node.AkkaMessaging
{
    public interface IHandlerActorTypeFactory
    {
        Type GetActorTypeFor(Type message, Type handler);
    }
}